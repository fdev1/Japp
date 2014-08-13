using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace JappUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
		
			/*
			//Fin * ((PLLDIV + 2) / ((PLLPRE + 2) * 2 * (PLLPOST + 1)))
			float fosc, bestFosc, newBestFosc = 80;// 140.0F;
			float plldiv, pllpre, pllpost;

		retryit:
			fosc = 0;
			bestFosc = newBestFosc;
			newBestFosc = 0;
			for (plldiv = 0; plldiv < 511; plldiv++)
			{
				for (pllpre = 0; pllpre < 31; pllpre++)
				{
					for (pllpost = 0; pllpost < 3; pllpost++)
					{
						fosc = (8 * ((plldiv + 2) / ((pllpre + 2) * 2 * (pllpost + 1))));
						if (fosc == bestFosc)
						{
							MessageBox.Show(string.Format("Fosc = {3}; PLLDIV={0}; PLLPRE={1}; PLLPOST={2}", plldiv, pllpre, pllpost, fosc));
							goto done;
						}
						else
						{
							if (fosc < bestFosc)
								newBestFosc = Math.Max(fosc, newBestFosc);
						}
					}
				}
			}
			done:
			if (fosc != bestFosc)
				goto retryit;
			 
			*/

			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
			catch (AppStartupException)
			{
				Application.Exit();
			}
        }
    }
}
