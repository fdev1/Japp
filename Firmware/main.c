#include <p18lf14k50.h>

#include "usb.h"
#include "usb_function_cdc.h"
#include "HardwareProfile.h"

/*
// options/config
*/
#define PP_SUPPORT_DIAG_COMMANDS
#define PP_COMM_TIMEOUT				1000u
#define PP_STATUS_LED				LATBbits.LATB5
#define PP_BUSY_LED					LATBbits.LATB7
#define PP_VPP						LATCbits.LATC6
#define PP_PGC						LATBbits.LATB6
#define PP_PGD						LATCbits.LATC7
#define PP_PGD_PIN					PORTCbits.RC7
#define PP_PGD_TRIS					TRISCbits.RC7
#define PP_HIGH						0
#define PP_LOW						1
#define PP_BUFFER_LEN				144
#define PP_VALIDATE_ARGUMENTS
#define PP_USE_MSSP

/*
// App commands
*/
#define PP_CMD_INIT					0x01
#define PP_CMD_PULSE_VPP_US			0x02
#define PP_CMD_HOLD_IN_RESET		0x03
#define PP_CMD_RELEASE_FROM_RESET	0x04
#define PP_CMD_SET_VDD				0x05
#define PP_CMD_SET_VPP				0x06
#define PP_CMD_WRITE				0x07
#define PP_CMD_READ					0x08
#define PP_CMD_SEND_PGC_CYCLES		0x09
#define PP_CMD_WRITE_BITS			0x0A
#define PP_CMD_SEND_5_NOPS			0x0B
#define PP_CMD_READ_TO_BUFFER		0x0C
#define PP_CMD_READ_BUFFER			0x0D
#define PP_CMD_GET_BUFFER_LEN		0x0E
#define PP_CMD_READ_VISI			0x0F
#define PP_CMD_READ_VISI_TO_BUFFER	0x10
#define PP_CMD_SET_SPI_5MHZ			0x11
#define PP_CMD_SET_SPI_1_85MHZ		0x12
#define PP_CMD_READ_SPI				0x13
#define PP_CMD_READ_TO_BUFFER_SPI	0x14
#define PP_CMD_TOGGLE_LED			0x7F
#define PP_CMD_RCON					0x80
#define PP_CMD_LAST_CMD				0x81


/*
// App responses
*/
#define PP_RSP_OK						0x01
#define PP_RSP_UNKNOWN_COMMAND			0x02
#define PP_RSP_COMMAND_NOT_SUPPORTED	0x03
#define PP_RSP_TIMEOUT					0x04
#define PP_RSP_INVALID_ARGUMENTS		0x05
#define PP_RSP_BUFFER_OVERFLOW			0x06

/*
// misc
*/
#define NOP()							_asm nop _endasm

/*
// reads count bytes from USBUSART into buffer[buffer_index]. uses
// num_bytes_read as temp storage
*/
#define PP_READ_ARGUMENTS(num_bytes_read, buffer, buffer_index, count)	\
	pp_comm_timeout = PP_COMM_TIMEOUT;									\
	num_bytes_read = 0;													\
	while (num_bytes_read < (count))									\
	{																	\
		num_bytes_read += getUSBUSART									\
		(																\
			&buffer[(buffer_index) + num_bytes_read], 					\
			(count) - (num_bytes_read)									\
		);																\
		if (!pp_comm_timeout)											\
		{																\
			while (!USBUSARTIsTxTrfReady())								\
				CDCTxService();											\
			byteTemp[0] = PP_RSP_TIMEOUT;								\
			byteTemp[1] = 0x0;											\
			putUSBUSART(byteTemp, 2);									\
			CDCTxService();												\
			PP_BUSY_LED = 0;											\
			return;														\
		}																\
	} 


/*
// mcu configuration bits
*/
#pragma config CPUDIV 	= NOCLKDIV
#pragma config USBDIV 	= OFF
#pragma config FOSC 	= HS
#pragma config PLLEN 	= ON
#pragma config PCLKEN 	= ON
#pragma config FCMEN 	= OFF
#pragma config IESO 	= OFF
#pragma config PWRTEN 	= OFF
#pragma config BOREN 	= OFF
#pragma config BORV 	= 22
#pragma config WDTEN 	= OFF
#pragma config WDTPS 	= 1
#pragma config HFOFST 	= OFF
#pragma config MCLRE 	= ON
#pragma config STVREN 	= OFF
#pragma config LVP 		= OFF

#pragma udata access accessram

/*
// access ram variables
*/
static near BYTE w0;
static near BYTE w1;
static near BYTE w2;
static near BYTE w3;
static near BYTE pp_buffer_index;	
static near BYTE pp_buffer_overflow_flag;

#pragma udata

/*
// global variables
*/
static WORD pp_comm_timeout;
static BYTE pp_buffer_data[PP_BUFFER_LEN + 2];
static BYTE* pp_buffer;

/*
// function prototypes
*/
static void pp_mcu_init(void);
static void pp_io_init(void);
static void pp_timers_init(void);
static void pp_mssp_init(void);
static void pp_toggle_status_led(void);
static void pp_tasks(void);
static void pp_sof_handler(void);
static void pp_idle(void);
static void pp_idle_resume(void);
static void pp_usb_error(void);
static void pp_write_byte(void);
static void pp_write_bits(void);
static void pp_send_5_nops(void);
static void pp_read_visi(void);
static BYTE pp_read_byte(void);
static BYTE pp_read_byte_spi(void);

#pragma code

/*
// application entry
*/
void main()
{
	/*
	// initialize buffer
	*/
	pp_buffer = &pp_buffer_data[2];
	pp_buffer_index = 0;
	pp_buffer_overflow_flag = 0;
	/*
	// initialize all hardware
	*/
	pp_mcu_init();			// system clock and interrupts
	pp_io_init();			// i/o ports
	pp_timers_init();		// timers
	pp_mssp_init();
    USBDeviceInit();		// usb device
	/*
	// jump to worker routine
	*/
	_asm goto pp_tasks _endasm
	pp_tasks();		// just to get rid of compiler warning
}

/*
// initialize system clock and interrupts
*/
static void pp_mcu_init(void)
{
	RCONbits.IPEN = 1;		// enable interrupt priorities
	INTCONbits.GIEH = 1;	// enable high priority interrupts
	INTCONbits.GIEL = 1;	// enable low priority interrupts
	INTCON2bits.TMR0IP = 0;	// timer0 priority: low
	IPR2bits.USBIP = 1;		// usb peripheral priority: high
}

/*
// initialize IO ports
*/
static void pp_io_init(void)
{
	/*
	// set all IO ports as digital
	*/
    ANSEL 	= 0x00;
	ANSELH 	= 0x00;
	/*
	// all IO pins are output except RB4
	*/
	TRISB 	= 0b00010000;	// RB5 = LED1 | RB7 = LED2 | RB4 = SDI | RB6 = SCK
	TRISC 	= 0b00000000;	// RC0:RC5 = NC | RC6 = VDD | RC7 = SDO
	/*
	// make sure VPP is LOW
	*/
	PP_VPP = PP_LOW;
	/*
	// set all IO pins low except RB5 (green LED)
	*/
	LATB 	= 0b00100000;
	LATC 	= 0b00000000;
}

/*
// configure hardware timers
*/
static void pp_timers_init(void)
{
	/*
	// configure timer0, we'll use it as a general
	// purpose timer
	*/
	T0CONbits.TMR0ON = 0;	// disabled
	T0CONbits.T08BIT = 0;	// 16-bit timer
	T0CONbits.T0CS = 0;		// instruction clock (FOSC/4)
	T0CONbits.T0SE = 0;		// increment on low to high trsansition
	T0CONbits.PSA = 1;		// enable pre-scaler
	T0CONbits.T0PS = 0b10;	// 1:4 (tick every 1/3uS)
	INTCONbits.TMR0IE = 1;	// enable timer1 interrupts
	/*
	// configure timer2, we'll use it to drive PWM (maybe)
	*/
	T2CONbits.TMR2ON = 0;	// disabled
	T2CONbits.T2CKPS = 0;	// no pre-scaler
	T2CONbits.T2OUTPS = 0;	// no output post-scale
	PR2 = 0xFF;				// period
}

/*
// initialize MSSP module for spi
*/
static void pp_mssp_init(void)
{
	SSPCON1bits.SSPEN = 0;		// disable MSSP
	SSPSTATbits.SMP = 1;		// sample at end of output time
	SSPSTATbits.CKE = 1;		// transmit on transition from active to idle
	SSPCON1bits.CKP = 0;		// idle = low
	SSPCON1bits.SSPM = 1;		// Fosc/16 (3MHz) // 0b11 = TMR2 Output / 2
}

/*
// app specific tasks
*/
static void pp_tasks(void)
{
    static BYTE numBytesRead;
	static BYTE byteTemp[5];

pp_loop_start:
	//w0 = 0;
	w1 = 0;
	//w2 = 0;
	//w3 = 0;
	/*
	// attach to the USB bus if not already done
	*/
	if(USB_BUS_SENSE && (USBGetDeviceState() == DETACHED_STATE))
	{
		USBDeviceAttach();
	}
	/*
	// check that the USB device is in the configured state
	*/
	if ((USBDeviceState < CONFIGURED_STATE) || (USBSuspendControl == 1))
	{
		/*
		// we'll toggle our status LED until the device
		// gets configured
		*/
		pp_toggle_status_led();
		/*
		// continue
		*/
		goto pp_loop_start;
	}
	/*
	// make sure status LED is lit
	*/
	PP_STATUS_LED = 1;
	/*
	// invoke TX service
	*/
	CDCTxService();
	/*
	// read the next command from USB
	*/
	numBytesRead = getUSBUSART(byteTemp, 1);
	/*
	// handle the command
	*/
	if(numBytesRead)
	{
		switch (byteTemp[0])
		{
			case PP_CMD_INIT:
				/*
				// initialize all IO's to know state
				*/
				pp_io_init();
				pp_mssp_init();
				/*
				// flush buffer
				*/
				pp_buffer_index = 0;
				pp_buffer_overflow_flag = 0;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			/*
			// sends a pulse on the MCLR/VPP pin
			//
			// ++++++++--------------------------------- CMD - Command byte
			// |||||||| ++++++++------------------------ LEN - Arguments length (always two)
			// |||||||| |||||||| ++++++++++++++++------- PWU - 16-bit pulse width (uS)
			// |||||||| |||||||| ||||||||||||||||
			// XXXXXXXX XXXXXXXX XXXXXXXXXXXXXXXX
			//
			// Conditions:
			// ===========
			// LEN = 2
			// 0 < PWD < 0x5555 (21845)
			// PWD is Little Endian
			*/
			case PP_CMD_PULSE_VPP_US:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// read arguments
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 3);
				/*
				// if the arguments length is not 2 return
				// PP_RSP_INVALID_ARGUMENTS
				*/
				if (byteTemp[0] != 2)
				{
					/*
					// busy-wait until we're ready to transmit data
					*/
					while (!USBUSARTIsTxTrfReady())
						CDCTxService();
					/*
					// transmit response
					*/
					byteTemp[0] = PP_RSP_INVALID_ARGUMENTS;
					byteTemp[1] = 0x0;
					putUSBUSART(byteTemp, 2);
					CDCTxService();
					PP_BUSY_LED = 0;
					goto pp_loop_start;
				}
				/*
				// make sure that the argument is within the
				// supported range (0x5555 or 21845)
				*/
				if (*((WORD*) &byteTemp[1]) == 0 || *((WORD*) &byteTemp[1]) > 0x5555)
				{
					/*
					// busy-wait until we're ready to transmit data
					*/
					while (!USBUSARTIsTxTrfReady())
						CDCTxService();
					/*
					// transmit response
					*/
					byteTemp[0] = PP_RSP_INVALID_ARGUMENTS;
					byteTemp[1] = 0x0;
					putUSBUSART(byteTemp, 2);
					CDCTxService();
					PP_BUSY_LED = 0;
					goto pp_loop_start;
				}
				/*
				// set the value of timer0 so that it overflows after the
				// specified amount of uSecs (PWU). The prescaler is set to 1:4
				// so at 12 mips it increments every .33us and it's set to
				// 16-bit so the value is 0xFFFF - (PWU * 3)
				//
				// TODO: adjust value to account for interrupt latency
				*/	
				*((WORD*) &byteTemp[1]) = 0xFFFF - (*((WORD*) &byteTemp[1]) * 3);
				TMR0H = byteTemp[2];
				TMR0L = byteTemp[1];
				/*
				// start timer0
				*/
				T0CONbits.TMR0ON = 1;
				/*
				// toggle VPP
				*/
				PP_VPP ^= 1;
				/*
				// wait for timer0 shut down, this is done by
				// the timer's ISR.
				*/
				while (T0CONbits.TMR0ON);
				/*
				// toggle VPP
				*/
				PP_VPP ^= 1;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response. 
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0;
				putUSBUSART(byteTemp, 2);
				PP_BUSY_LED = 0;
				break;

			/*
			// sets VPP/MCLR to VSS
			*/
			case PP_CMD_HOLD_IN_RESET:
				/*
				// drive MCLR/VPP low
				*/
				PP_VPP = PP_LOW;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// transmit response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			/*
			// sets VPP/MCLR to VDD
			*/
			case PP_CMD_RELEASE_FROM_RESET:
				// todo: make sure vpp = vdd
				/*
				// drive MCLR/VPP high
				*/
				PP_VPP = PP_HIGH;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// transmit response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			case PP_CMD_SET_VDD:
				/*
				// read argument
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 2);
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send command not supported response
				*/
				byteTemp[0] = PP_RSP_COMMAND_NOT_SUPPORTED;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			case PP_CMD_SET_VPP:
				/*
				// read argument
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 2);
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send command not supported response
				*/
				byteTemp[0] = PP_RSP_COMMAND_NOT_SUPPORTED;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			case PP_CMD_WRITE:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// reset the timeout counter
				*/
				pp_comm_timeout = PP_COMM_TIMEOUT;
				/*
				// read argument (len)
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 1, 1);
				/*
				// copy length argument to access ram
				*/
				w0 = byteTemp[1];
				/*
				// set PGD as output
				*/
				PP_PGD_TRIS = 0;
				#if defined(PP_USE_MSSP)
				SSPCON1bits.SSPEN = 1;
				#endif
				/*
				// clock data out of spi
				*/
				while (w0)
				{
					/*
					// reset the timeout counter
					*/
					pp_comm_timeout = PP_COMM_TIMEOUT;
					/*
					// read the next byte from USB
					*/
					do
					{
						numBytesRead = getUSBUSART(byteTemp, 1);
						/*
						// if the timeout has been reached return
						// timeout response
						*/
						if (!pp_comm_timeout)
						{
							/*
							// busy-wait until we're ready to transmit data
							*/
							while (!USBUSARTIsTxTrfReady())
								CDCTxService();
							/*
							// transmit response
							*/
							byteTemp[0] = PP_RSP_TIMEOUT;
							byteTemp[1] = 0x0;
							putUSBUSART(byteTemp, 2);
							CDCTxService();
							PP_BUSY_LED = 0;
							goto pp_loop_start;
						}
					}
					while (!numBytesRead);
					/*
					// write byte to device
					*/
					w2 = byteTemp[0];
					pp_write_byte();
					/*
					// decrement counter
					*/
					w0--;
				}
				/*
				// clear led
				*/
				PP_BUSY_LED = 0;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;
			
			case PP_CMD_READ_SPI:
				w1 = 1;
			case PP_CMD_READ:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// read argument (len)
				// reaad 2 bytes into byteTemp[0]
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 2);
				/*
				// make sure the 1st argumment is 1
				*/
				#if defined(PP_VALIDATE_ARGUMENTS)
				if (byteTemp[0] != 1)
				{
					/*
					// reset the timeout counter
					*/
					pp_comm_timeout = PP_COMM_TIMEOUT;
					/*
					// flush any extra arguments
					*/
					numBytesRead = 0;
					while (numBytesRead < byteTemp[0] - 1 || !pp_comm_timeout)
					{
						numBytesRead += getUSBUSART(&byteTemp[1], 1);
					}
					/*
					// busy-wait until we're ready to transmit data
					*/
					while (!USBUSARTIsTxTrfReady())
						CDCTxService();
					/*
					// transmit response
					*/
					byteTemp[0] = PP_RSP_INVALID_ARGUMENTS;
					byteTemp[1] = 0x0;
					putUSBUSART(byteTemp, 2);
					goto pp_loop_start;
				}
				#endif
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// return OK response followed by the amount
				// of bytes in the response (which is the same as the
				// amount of bytes in the input.)
				*/
				byteTemp[0] = PP_RSP_OK;
				//byteTemp[1] = len; ^^^^
				putUSBUSART(byteTemp, 2);
				CDCTxService();
				/*
				// enable MSSP
				*/
				#if defined(PP_USE_MSSP)
				SSPCON1bits.SSPEN = 1;
				#endif
				/*
				// copy length to near memory
				*/
				w0 = byteTemp[1];
				/*
				// now we'll read the input, clock it out the
				// SPI and transmit whatever we get from the SPI
				// back to the host.
				*/
				if (w1)
				{
					/*
					// enable MSSP
					*/
					#if !defined(PP_USE_MSSP)
					SSPCON1bits.SSPEN = 1;
					#endif
					/*
					// read bytes
					*/
					while (w0--)
					{
						/*
						// read a byte from the PGD line.
						*/
						byteTemp[0] = pp_read_byte_spi();
						/*
						// busy-wait until we're ready to transmit data
						*/
						while (!USBUSARTIsTxTrfReady())
							CDCTxService();
						/*
						// send the byte to host
						*/
						putUSBUSART(byteTemp, 1);
					}
					/*
					// disable MSSP
					*/
					#if !defined(PP_USE_MSSP)
					SSPCON1bits.SSPEN = 1;
					#endif
				}
				else
				{
					while (w0--)
					{
						/*
						// read a byte from the PGD line.
						*/
						byteTemp[0] = pp_read_byte();
						/*
						// busy-wait until we're ready to transmit data
						*/
						while (!USBUSARTIsTxTrfReady())
							CDCTxService();
						/*
						// send the byte to host
						*/
						putUSBUSART(byteTemp, 1);
					}
				}
				/*
				// clear the busy LED
				*/
				PP_BUSY_LED = 0;
				break;
			/*
			// this commmand sends no response
			*/
			case PP_CMD_READ_TO_BUFFER_SPI:
				w1 = 1;
			case PP_CMD_READ_TO_BUFFER:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// read argument (len)
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 2);
				/*
				// copy length to near memory
				*/
				w0 = byteTemp[1];
				/*
				// if the buffer has been overflown or would be overflown
				// by this command return without doing any work
				*/
				if (pp_buffer_overflow_flag || w0 > PP_BUFFER_LEN || pp_buffer_index + w0 > PP_BUFFER_LEN)
				{
					pp_buffer_overflow_flag = 1;
					PP_BUSY_LED = 0;
					goto pp_loop_start;
				}
				/*
				// enable MSSP
				*/
				#if defined(PP_USE_MSSP)
				SSPCON1bits.SSPEN = 1;
				#endif
				/*
				// now we'll read the input, clock it out the
				// SPI and transmit whatever we get from the SPI
				// back to the host.
				*/
				if (w1)
				{
					/*
					// enable MSSP
					*/
					#if !defined(PP_USE_MSSP)
					SSPCON1bits.SSPEN = 1;
					#endif
					/*
					// read bytes
					*/
					while (w0--)
					{
						/*
						// read a byte from the PGD line.
						*/
						pp_buffer[pp_buffer_index++] = pp_read_byte_spi();
					}
					/*
					// disable MSSP
					*/
					#if !defined(PP_USE_MSSP)
					SSPCON1bits.SSPEN = 0;
					#endif
				}
				else
				{
					while (w0--)
					{
						/*
						// read a byte from the PGD line.
						*/
						pp_buffer[pp_buffer_index++] = pp_read_byte();
					}
				}	
				/*
				// clear the busy LED
				*/
				PP_BUSY_LED = 0;
				break;

			case PP_CMD_READ_BUFFER:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// return OK response followed by buffer contents
				*/
				if (pp_buffer_overflow_flag)
				{
					byteTemp[0] = PP_RSP_BUFFER_OVERFLOW;
					byteTemp[1] = 0;
					putUSBUSART(byteTemp, 2);
				}
				else
				{
					pp_buffer_data[0] = PP_RSP_OK;
					pp_buffer_data[1] = pp_buffer_index;
					putUSBUSART(pp_buffer_data, pp_buffer_index + 2);
					/*
					// invoke TX service
					*/
					CDCTxService();
				}
				/*
				// resest buffer index
				*/
				pp_buffer_index = 0;
				pp_buffer_overflow_flag = 0;
				/*
				// clear the busy LED
				*/
				PP_BUSY_LED = 0;
				break;

			case PP_CMD_READ_VISI:
				/*
				// read VISI into w0:w1
				*/
				pp_read_visi();
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send the response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x2;
				byteTemp[2] = w0;
				byteTemp[3] = w1;
				putUSBUSART(byteTemp, 4);
				PP_BUSY_LED = 0;
				break;		

			case PP_CMD_READ_VISI_TO_BUFFER:
				/*
				// light up the busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// if the buffer has been overflown or would be overflown
				// by this command return without doing any work
				*/
				if (pp_buffer_overflow_flag || pp_buffer_index + 2 > PP_BUFFER_LEN)
				{
					pp_buffer_overflow_flag = 1;
					PP_BUSY_LED = 0;
					goto pp_loop_start;
				}
				/*
				// read VISI into w0:w1
				*/
				pp_read_visi();
				/*
				// read VISI to buffer
				*/
				pp_buffer[pp_buffer_index++] = w0;
				pp_buffer[pp_buffer_index++] = w1;
				/*
				// clear busy led
				*/
				PP_BUSY_LED = 0;
				break;	

			case PP_CMD_GET_BUFFER_LEN:
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send the OK response followed by the contents
				// of the RCON register
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x1;
				byteTemp[2] = PP_BUFFER_LEN;
				putUSBUSART(byteTemp, 3);
				PP_BUSY_LED = 0;
				break;
			/*
			// send a specified number of cycles on the SCK
			// line. The format for this command is as follows:
			//
			// ++++++++-------------------------- CMD - Command byte
			// |||||||| ++++++++----------------- LEN - Arguments length (always 1)             
			// |||||||| |||||||| ++++++++-------- CNT - The number of cycles to send
			// |||||||| |||||||| ||||||||
			// XXXXXXXX XXXXXXXX XXXXXXXX
			*/
			case PP_CMD_SEND_PGC_CYCLES:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// read arguments (len/cnt)
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 2);
				/*
				// make sure that we received the correct length
				// parameter
				*/
				if (byteTemp[0] != 1)
				{
					/*
					// if the user is sending extra bytes flush the buffer
					*/
					if (byteTemp[0] > 1 && numBytesRead < (byteTemp[0] + 1))
					{
						getsUSBUSART(&byteTemp[3], 1);
					}
					/*
					// busy-wait until we're ready to transmit data
					*/
					while (!USBUSARTIsTxTrfReady())
						CDCTxService();
					/*
					// and send error code
					*/
					byteTemp[0] = PP_RSP_INVALID_ARGUMENTS;
					byteTemp[1] = 0x0;
					putUSBUSART(byteTemp, 2);
					CDCTxService();
					PP_BUSY_LED = 0;
					goto pp_loop_start;
				}
				/*
				// disable mssp module and drive SDO low
				*/
				PP_PGD = 0;
				PP_PGD_TRIS = 0;
				/*
				// disable MSSP
				*/
				#if defined(PP_USE_MSSP)
				SSPCON1bits.SSPEN = 0;
				#endif
				/*
				// copy counter to near memory
				*/
				w0 = byteTemp[1];
				/*
				// clock-out the requested # of cycles
				*/
				while (w0)
				{
					PP_PGC = 1;
					w0--;
					PP_PGC = 0;
				}
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				PP_BUSY_LED = 0;
				break;

			case PP_CMD_WRITE_BITS:
				/*
				// light up busy led
				*/
				PP_BUSY_LED = 1;
				/*
				// read argument (len)
				*/
				PP_READ_ARGUMENTS(numBytesRead, byteTemp, 0, 3);
				/*
				// check that byte arg0 (len) = 2 and arg1 (bit cnt) <= 8
				*/
				if (byteTemp[0] != 2 || byteTemp[1] > 8)
				{
					/*
					// busy-wait until we're ready to transmit data
					*/
					while (!USBUSARTIsTxTrfReady())
						CDCTxService();
					/*
					// and send error code
					*/
					byteTemp[0] = PP_RSP_INVALID_ARGUMENTS;
					byteTemp[1] = 0x0;
					putUSBUSART(byteTemp, 2);
					CDCTxService();
					PP_BUSY_LED = 0;
					goto pp_loop_start;
				}
				/*
				// disable mssp module and drive SDO low
				*/
				PP_PGD = 0;
				PP_PGD_TRIS = 0;
				/*
				// disable MSSP
				*/
				#if defined(PP_USE_MSSP)
				SSPCON1bits.SSPEN = 0;
				#endif
				/*
				// write bits
				*/
				w0 = byteTemp[1];
				w1 = byteTemp[2];
				pp_write_bits();
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				PP_BUSY_LED = 0;
				break;
			
			case PP_CMD_SEND_5_NOPS:
				/*
				// disable MSSP
				*/
				#if defined(PP_USE_MSSP)
				SSPCON1bits.SSPEN = 0;
				#endif
				/*
				// send 5 nops
				*/
				pp_send_5_nops();
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;
				
			case PP_CMD_SET_SPI_5MHZ:
				/*
				// set the MSSP module to 3MHz. TODO: see if we can do
				// better with Timer2 as clock source
				*/
				w0 = SSPCON1bits.SSPEN;
				SSPCON1bits.SSPM = 1;	/* (Fosc / 16) = (48Mhz / 16) = 3Mhz */
				SSPCON1bits.SSPEN = w0;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;
				
			case PP_CMD_SET_SPI_1_85MHZ:
				/*
				// set MSSP module at .75 MHz. TODO: see if we can do better with
				// Timer2 as clock source
				*/
				w0 = SSPCON1bits.SSPEN;
				SSPCON1bits.SSPM = 2;	/* (Fosc / 64) = (48Mhz / 64) = .75 Mhz */
				SSPCON1bits.SSPEN = w0;
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			#if defined(PP_SUPPORT_DIAG_COMMANDS)
			case PP_CMD_TOGGLE_LED:
				/*
				// toggle the busy LED 10 times in 1 sec
				*/					
				for (byteTemp[0] = 0; byteTemp[0] < 10; byteTemp[0]++)
				{
					pp_comm_timeout = 100;
					while (pp_comm_timeout);
					PP_BUSY_LED ^= 1;
				}
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send OK response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;

			case PP_CMD_RCON :
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send the OK response followed by the contents
				// of the RCON register
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x1;
				byteTemp[2] = RCON;
				putUSBUSART(byteTemp, 3);
				break;

			case PP_CMD_LAST_CMD:
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// send response
				*/
				byteTemp[0] = PP_RSP_OK;
				byteTemp[1] = 0x1;
				byteTemp[2] = 0x0; //lastCommand[1];
				putUSBUSART(byteTemp, 3);
			#endif

			default :
				/*
				// busy-wait until we're ready to transmit data
				*/
				while (!USBUSARTIsTxTrfReady())
					CDCTxService();
				/*
				// transmit response command
				*/
				byteTemp[0] = PP_RSP_UNKNOWN_COMMAND;
				byteTemp[1] = 0x0;
				putUSBUSART(byteTemp, 2);
				break;
		}
	}
	/*
	// continue processing commands
	*/
	goto pp_loop_start;
}

/*
// read VISI into w0:w1
*/
static void pp_read_visi(void)
{
	/*
	// disable MSSP module
	*/
	#if defined(PP_USE_MSSP)
	SSPCON1bits.SSPEN = 0;
	#endif
	/*
	// set PGD as output
	*/
	PP_PGD = 0;
	PP_PGD_TRIS = 0;
	/*
	// send REGOUT command
	*/
	w0 = 0x04;
	w1 = 0x80;
	pp_write_bits();
	/*
	// wait P4 (40ns)
	*/
	NOP();
	/*
	// enable MSSP
	*/
	#if defined(PP_USE_MSSP)
	SSPCON1bits.SSPEN = 1;
	#endif
	/*
	// send 8 clock cycles
	*/
	w2 = 0x00;
	pp_write_byte();
	/*
	// wait P5 (20ns)
	*/
	NOP();
	/*
	// read VISI
	*/
	w0 = pp_read_byte();
	w1 = pp_read_byte();
}

/*
// write bits
// w0 = count
// w1 = data
*/
static void pp_write_bits(void)
{
	/*
	// clock-out the requested # of bits
	*/
	while (w0)
	{
		/*
		// set SDO to the MSB of the buffered
		// byte and shift the bit out
		*/
		PP_PGD = ((w1 & 0x80) > 0);
		/*
		// toggle PGC
		*/
		PP_PGC = 1;
		/*
		// decrement the bit counters and shift
		// one bit out
		*/
		w1 <<= 1;
		w0--;
		/*
		// toggle SCK
		*/
		PP_PGC = 0;
	}
}

/*
// write a byte to PGD
// w2 = byte
*/
static void pp_write_byte(void)
{
	#if defined(PP_USE_MSSP)
	SSPBUF = w2;
	while (!SSPSTATbits.BF);
	w3 = SSPBUF;
	#else
	/*
	// copy byte to near memory
	*/
	w3 = w2 & 0b10000000;
	/*
	// bit 7
	*/
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b01000000;
	PP_PGC = 0;
	//
	// bit 6
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b00100000;
	PP_PGC = 0;
	//
	// bit 5
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b00010000;
	PP_PGC = 0;
	//
	// bit 4
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b00001000;
	PP_PGC = 0;
	//
	// bit 3
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b00000100;
	PP_PGC = 0;
	//
	// bit 2
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b00000010;
	PP_PGC = 0;
	//
	// bit 1
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	w3 = w2 & 0b00000001;
	PP_PGC = 0;
	//
	// bit 0
	//
	PP_PGD = (w3 != 0);
	PP_PGC = 1;
	NOP();
	NOP();
	PP_PGC = 0;
	#endif
}


static BYTE pp_read_byte_spi(void)
{
	SSPBUF = 0xFF;
	while (!SSPSTATbits.BF);
	return SSPBUF;
}

/* 
// reads a byte from PGD
*/
static BYTE pp_read_byte(void)
{
	#if defined(PP_USE_MSSP)
	SSPBUF = 0xFF;
	while (!SSPSTATbits.BF);
	w3 = SSPBUF;
	w3 = ((w3 * 0x0802LU & 0x22110LU) | (w3 * 0x8020LU & 0x88440LU)) * 0x10101LU >> 16;
	#else
	PP_PGD_TRIS = 1;
	/*
	// bit 7
	*/
	w3 = 0;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 6
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 5
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 4
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 3
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 2
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 1
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	/*
	// bit 0
	*/
	w3 >>= 1;
	PP_PGC = 1;
	if (PP_PGD_PIN)
		w3 |= 0x80;
	PP_PGC = 0;
	#endif
	/*
	// return byte
	*/
	return w3;
}

#define PP_SEND_BIT(b)		\
	PP_PGD = b;				\
	PP_PGC = 1;				\
	NOP();					\
	PP_PGC = 0

#define PP_SEND_NOP()		\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0);			\
	PP_SEND_BIT(0)

static void pp_send_5_nops(void)
{
	PP_PGD = 0;
	PP_PGD_TRIS = 0;
	/*
	// SIX
	*/
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	/*
	// bset NVMCON, #WR
	*/
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	PP_SEND_BIT(1);
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	PP_SEND_BIT(1);
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	PP_SEND_BIT(0);
	PP_SEND_BIT(0);
	PP_SEND_BIT(1);
	/*
	// send 5 nops
	*/
	PP_SEND_NOP();
	PP_SEND_NOP();
	PP_SEND_NOP();
	PP_SEND_NOP();
	PP_SEND_NOP();
}

static void pp_toggle_status_led(void)
{
	static char i = 0, b = 0;

	if (b == 0x1F)
	{
		b = 0;
		if (i == 0xFF)
		{
			i = 0;
			PP_STATUS_LED ^= 1;
		}
		else 
		{
			i++;
		}
	}
	else
	{
		b++;
	}
}

/*
// usb SOF event handler
*/
static void pp_sof_handler(void)
{
	/*
	// if the communication timeout count is running
	// decrement it. TODO: it may be best to use a hardware
	// timer for this.
	*/
	if (pp_comm_timeout)
		pp_comm_timeout--;
}

/*
// sets the device in idle mode
*/
static void pp_idle(void)
{

}

/*
// sets the device in normal mode
*/
static void pp_idle_resume(void)
{

}

/*
// usb error handler
*/
static void pp_usb_error(void)
{

}

/*
// USB event handler
*/
BOOL USER_USB_CALLBACK_EVENT_HANDLER(int event, void *pdata, WORD size)
{
    switch (event)
    {
        case EVENT_SOF : pp_sof_handler(); break;
        case EVENT_SUSPEND : pp_idle(); break;
        case EVENT_RESUME : pp_idle_resume(); break;
        case EVENT_CONFIGURED : CDCInitEP(); break;
        case EVENT_EP0_REQUEST : USBCheckCDCRequest(); break;
        case EVENT_BUS_ERROR : pp_usb_error(); break;
        default : break;
    }      
    return TRUE; 
}

/*
// high priority isr
*/
#pragma interrupt high_isr
void high_isr(void)
{
	/*
	// service usb interrups
	*/
   	USBDeviceTasks();
}

/*
// low priority isr
*/
#pragma interruptlow low_isr
void low_isr(void)
{
	/*
	// service timer0 interrupt
	*/
	if (INTCONbits.TMR0IF == 1)
	{
		INTCONbits.TMR0IF = 0;
		T0CONbits.TMR0ON = 0;
	}
}

/*
// high priority interrupt vector
*/ 
#pragma code high_vector=0x08
void high_vector(void)
{
	_asm GOTO high_isr _endasm
}

/*
// low priority interrupt vector
*/
#pragma code low_vector=0x18
void low_vector(void)
{
	_asm GOTO low_isr _endasm
}
