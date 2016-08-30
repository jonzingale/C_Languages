#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization; 
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
/// <summary>
/// BBSbywesDiver is a BBsqueeze indicator  by Wes Pittman. I took John Carter's idea and coded this little ditty with many extra bells and whistles
/// </summary>
    [Description("BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles")]
    public class BBSbywesDivLiveTC : Indicator
    {
        #region Variables
	// BB variables
            private int 			bBperiod 	= 20;			// Default setting for BBperiod
            private double 			bBdevi 		= 2.0;		// Default setting for Bbdevi
			private int				signalOffset= 2;
		
			private DataSeries			bbupper;
			private DataSeries			bbmiddle;
			private DataSeries			bblower;


			private bool showBB			= false;


	// KC variables
            private int 			kCperiod 	= 20;			// Default setting for KCperiod
            private double 			kCoffset 	= 1.5;		//setting for KCoffset
			private int				kCATRperiod  = 10;		// D s f KCATRrange

			private DataSeries			kcupper;
			private DataSeries			kcmiddle;
			private DataSeries			kclower;

			private DataSeries			kcAvg;
			private DataSeries			atrValue;
		
			private bool showKC			= false;


	// SMI variables
			private int				percentDLength	= 3;	   // Dperiod
			private int				percentKLength	= 5;	   // Kperiod
			private DataSeries			rel_diff;
			private DataSeries			diff;



	//  divergence variables

			private double 		percentBarComplete	= .75;
			private int 		lastBar				= 1;

			private double 		TestPeaksmi			= 0;
			private int			PeakBarsAgo			= 0;
			private double 		LastPeaksmi			= 0;
			private double 		PrevPeaksmi			= 0;
			private double 		ThirdPeaksmi		= 0;
			private double 		LastPeakPrice		= 0;
			private double 		PrevPeakPrice		= 0;
			private double 		ThirdPeakPrice		= 0;
			private int			LastPeakBar			= 0;
			private int			PrevPeakBar			= 0;
			private int		ThirdPeakBar			= 0;
			private int		LastPeakBarsAgo			= 0;
			private int		PrevPeakBarsAgo			= 0;
			private int		ThirdPeakBarsAgo		= 0;
			
			private double 		TestVal				= 0;
			private int		ValBarsAgo				= 0;
			private double 		LastValsmi			= 0;
			private double 		PrevValsmi			= 0;
			private double 		ThirdValsmi			= 0;
			private double 		LastValPrice		= 0;
			private double 		PrevValPrice		= 0;
			private double 		ThirdValPrice		= 0;
			private int		LastValBar				= 0;
			private int		PrevValBar				= 0;
			private int		ThirdValBar				= 0;
			private int		LastValBarsAgo			= 0;
			private int		PrevValBarsAgo			= 0;
			private int		ThirdValBarsAgo			= 0;
			
			private int Crossbar	= 0;
			private int	Bmult	= 0;


		
		
        #endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			Add(new Plot(Color.FromKnownColor(KnownColor.HotPink), PlotStyle.Line, "smi"));					// 0
			Add(new Plot(Color.FromKnownColor(KnownColor.Silver), PlotStyle.Bar, "AvgSMI"));				// 1
			Add(new Plot(Color.FromKnownColor(KnownColor.Silver), PlotStyle.Dot, "ZeroLine"));			// 2
			Add(new Plot(Color.FromKnownColor(KnownColor.Transparent), PlotStyle.TriangleUp, "BBbandTop"));		// 3        ditched     2013 12 20
			Add(new Plot(Color.FromKnownColor(KnownColor.Transparent), PlotStyle.TriangleDown, "BBbandBottom"));	// 4        ditched     2013 12 20

			Add(new Plot(Color.Transparent, "BBUpper band"));		// 5         not plotting just yet, better figure out how to switch first
			Add(new Plot(Color.Transparent, "BBMedian"));	// 6    hope it doesn't screw up the indi window...
			Add(new Plot(Color.Transparent, "BBLower band"));		// 7

			Add(new Plot(Color.Transparent, "KCUpper band"));		// 8         not plotting just yet, better figure out how to switch first
			Add(new Plot(Color.Transparent, "KCMedian"));	// 9    hope it doesn't screw up the indi window...
			Add(new Plot(Color.Transparent, "KCLower band"));		// 10

			Add(new Plot(Color.FromKnownColor(KnownColor.Transparent), PlotStyle.Dot, "KCohcTop"));		// 11           ditched     2013 12 20
			Add(new Plot(Color.FromKnownColor(KnownColor.Transparent), PlotStyle.Dot, "KColcBot"));	// 12               ditched     2013 12 20

			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Dot, "CloseAboveMean"));		// 13
			Add(new Plot(Color.FromKnownColor(KnownColor.Green), PlotStyle.Dot, "CloseBelowMean"));		// 14
			
			Add(new Line(Color.FromKnownColor(KnownColor.Red), 80, "Eighty"));
			Add(new Line(Color.FromKnownColor(KnownColor.LimeGreen), -80, "Eightyneg"));
			Add(new Line(Color.FromKnownColor(KnownColor.Silver), 20, "Twenty"));
			Add(new Line(Color.FromKnownColor(KnownColor.Silver), -20, "Twentyneg"));
			Add(new Line(Color.FromKnownColor(KnownColor.YellowGreen), 40, "Forty"));
			Add(new Line(Color.FromKnownColor(KnownColor.Red), -40, "Fortyneg"));
			Add(new Line(Color.FromKnownColor(KnownColor.Gainsboro), 0, "Zero"));
			
			Plots[0].Pen.Width = 3;
			Plots[1].Pen.Width = 6;
			Plots[2].Pen.Width = 3;
			Plots[3].Pen.Width = 3;
			Plots[4].Pen.Width = 3;
			Plots[11].Pen.Width = 1;
			Plots[12].Pen.Width = 1;
			Plots[13].Pen.Width = 1;
			Plots[14].Pen.Width = 1;
			
			Lines[2].Pen.DashStyle = DashStyle.Dash;
			Lines[3].Pen.DashStyle = DashStyle.Dash;
			
			PlotsConfigurable = true;
			
		//stochastic momentums

			rel_diff	= new DataSeries(this);	// this refers to the indicator itself and syncs the DataSeries object to historical bars in the primary bars object
			diff		= new DataSeries(this);	// sincronizes dataSeries to primary bar series


		//bollinger bands

			bbupper 	= new DataSeries(this);			// sincronizes dataSeries to chart primary bar series
			bbmiddle 	= new DataSeries(this);
			bblower		= new DataSeries(this);

		//keltner

			kcAvg		= new DataSeries(this);
			atrValue	= new DataSeries(this);
	
			kcupper		= new DataSeries(this);
			kcmiddle	= new DataSeries(this);
			kclower		= new DataSeries(this);
		
			
			CalculateOnBarClose			= false;			
			Overlay					= false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
    	protected override void OnBarUpdate()
    	{
			if(CurrentBar < KCperiod || CurrentBar < KCATRperiod) return;

		
			ZeroLine.Set		(0);
//			BBbandTop.Set		(110);
//			BBbandBottom.Set 	(-110);
//
//			KCohcTop.Set		(110);
//			KColcBot.Set		(-110);
			
			Values[13].Set	(60);
			Values[14].Set	(-60);
			

		// SMI plots calculations

			double min_low = MIN(Low, percentKLength)[0];
			double max_high = MAX(High, percentKLength)[0];
			rel_diff.Set(Close[0] - (max_high + min_low)/2);
			diff.Set(max_high - min_low);

			double avgrel = EMA(EMA(rel_diff, percentDLength), percentDLength)[0];
			double avgdiff = EMA(EMA(diff, percentDLength), percentDLength)[0];

			smi.Set(avgrel/(avgdiff/2)*100);              // PROBABLY SHOULD BE LOWER CASE DUE TO @SMI.cs 
			AvgSMI.Set(EMA(smi, percentDLength)[0]);


		// end SMI plot calcs



		// BBband (not plotting) calculations
			
			double smaValue    = SMA(BBperiod)[0];
			double stdDevValue = StdDev(BBperiod)[0];

// probably have to make new plot names or change bottom indicator names

			bbupper.Set(smaValue + bBdevi * stdDevValue);
			bbmiddle.Set(smaValue);
			bblower.Set(smaValue - bBdevi * stdDevValue);


//			if (showBB)
//			{	BBUpper.Set (bbupper);
//				BBMiddle.Set (bbmiddle);
//				BBLower.Set (bblower);	}
//			else
//			{	BBUpper.Reset();
//				BBMiddle.Reset();
//				BBLower.Reset();	}


		// end BBband (not plotting) calculations

		//  KeltnerChannell calculations

			double kcAvg = SMA(KCperiod)[0];
			double atrValue = ATR(KCATRperiod)[0];
			
			// probably have to make new plot names or change bottom indicator names

			kcmiddle.Set (kcAvg);
			kcupper.Set  (kcAvg + (KCoffset * atrValue));
			kclower.Set  (kcAvg - (KCoffset * atrValue));

//			if (showKC)
//			{	KCUpper.Set (kcupper);
//				KCMiddle.Set (kcmiddle);
//				KCLower.Set (kclower);	}
//			else
//			{	KCUpper.Reset();
//				KCMiddle.Reset();
//				KCLower.Reset();	}


			
		//  end KeltnerChannell calculations

		// SMI color changer
			
			if ( smi[0] > smi[1] )
				{ PlotColors[0][0] = Color.Lime; }
//			if ( smi[0] < smi[1] )
			else
				{ PlotColors[0][0] = Color.DeepPink; }
//			else
//				{ PlotColors[0][0] = Color.Yellow; }
			
		// end SMI color changer
			
		//  SMI/AvgSMIBAR color change
			
			if ( smi[0] > 0.0 )
				{ if ( smi[0] > AvgSMI[0] )
					{  PlotColors[1][0] = Color.Cyan;}
					else if ( smi[0] < AvgSMI[0] )
					{  PlotColors[1][0] = Color.BlueViolet;}
					else
					{  PlotColors[1][0] = Color.PaleGreen;}
				}	
			if ( smi[0] < 0.0 )
				{ if ( smi[0] < AvgSMI[0] )
					{  PlotColors[1][0] = Color.Red;}
					else if ( smi[0] > AvgSMI[0] )
					{  PlotColors[1][0] = Color.Gold;}
					else
					{  PlotColors[1][0] = Color.Pink;}
				}

		// end SMI/AvgSMI BAR color change

		// Zero line signal color changer    plot 2                       // could change to BoolSeries pinch = this

			if ( bbupper[0] > kcupper[0] && bblower[0] < kclower[0] )    //   CHANGED 2013 12 20 - - removed =sign   (was >= and <=)    (now > and < only)
				{ PlotColors[2][0] = Color.LimeGreen;
				  BackColor = Color.Empty;  }
				else
				{ PlotColors[2][0] = Color.Maroon;
				  BackColor	= Color.FromArgb(80,0,0); }

		// end Zero line signal color change

		// CloseAboveMean CloseBelowMean
			
			if ( (bbupper[0] < kcupper[0]) && (bblower[0] > kclower[0]) && ( Close[0] > bbmiddle[0] ) )
					{ PlotColors[13][0] = Color.Cyan; }
				else if ( ( ( bbupper[0] <= kcupper[0] ) || ( bblower[0] >= kclower[0] ) )                 // includes incomplete pinches
							&& ( Close[0] >= bbmiddle[0] ) )    
					{ PlotColors[13][0] = Color.Turquoise; }
				else
					{PlotColors[13][0] = Color.Transparent;}

			if ( (bbupper[0] < kcupper[0]) && (bblower[0] > kclower[0]) && ( Close[0] < bbmiddle[0] ) )
					{ PlotColors[14][0] = Color.Gold; }
				else if ( ( ( bbupper[0] <= kcupper[0] ) || ( bblower[0] >= kclower[0] ) )               // includes incomplete pinches
							&& ( Close[0] <= bbmiddle[0] ) )    
					{ PlotColors[14][0] = Color.Orange; }

				else
					{PlotColors[14][0] = Color.Transparent;}
					
		// BBbandTop and BBbandBottom outside Keltner signal   plots 3 top and 4 bottom

//			if ( bbupper[0] >= kcupper[0] )
//				{ PlotColors[3][0] = Color.LimeGreen; }
//				else
//				{ PlotColors[3][0] = Color.Maroon; }
//			if ( bblower[0] <= kclower[0] )
//				{ PlotColors[4][0] = Color.LimeGreen; }
//				else
//				{ PlotColors[4][0] = Color.Maroon; }

		
		// end BBbandTop and BBbandBottom outside Keltner signal
				
		
		// CLOSE ABOVE KC || CLOSE BELOW KC               used to be HC > upper and LC < lower changed to just CLOSES
// changed it to H n C above and L n C below

//			if (  Close[0] > kcupper[0] ) 					// High[0] > kcupper[0] &&
//				{ PlotColors[11][0] = Color.LimeGreen; }
//				else
//				{ PlotColors[11][0]= Color.Transparent;}
//			if ( Close[0] < kclower[0] ) 					// Low[0] < kclower[0] && 
//				{ PlotColors[12][0]=  Color.Red;}
//				else
//				{ PlotColors[12][0]= Color.Transparent;}



		// end OPEN HIGH AND CLOSE ABOVE KC || OPEN LOW AND CLOSE BELOW KC
	// END main indiator calcs BBSbywes


// divergence calcs

	// percent Bar Complete - added to avoid divergence calcs checking every fucking tic and also to allow something other than OnFirstTickOfBar because it didn't work - much
	// divergence calcs now happen only once - once percentBarComplete has been satisfied	
		

					
		if( CurrentBar > BBperiod)
		{
			if (Bars.PercentComplete >= PercentBarComplete )   //&& lastBar != CurrentBar)    ///   percentBarComplete counter calculates divergence once after bar has built up to or through PercentBarComplete value
			{

		// Check CCI indicator for Divergence with Price data
		
	
	/*-------------------------------------------------------------------------------------------------------------			
				 Determine LAST PEAK in the SMI Line, record values for it and previous peak
	-------------------------------------------------------------------------------------------------------------*/

			//Only Look for Peaks in SMI Line
 
				if( smi[0] < smi[1] && smi[1] > smi[2] )  //  two lower b4 peak // && smi[2] > smi[3]) 
				{

				//Only take peaks that are above smi value of 5 

					if(smi[1] > 9.9999)              // using even micro momentums of 9.99999	
					{
											
					//	Print("----------------------------------- ");
					//	Print("TestPeak= "+TestPeaksmi);
					
						ThirdPeaksmi = PrevPeaksmi;
						ThirdPeakPrice = PrevPeakPrice;
						ThirdPeakBar = PrevPeakBar;
						ThirdPeakBarsAgo = (CurrentBar - ThirdPeakBar);
					
						PrevPeaksmi = LastPeaksmi;
						PrevPeakPrice = LastPeakPrice;
						PrevPeakBar = LastPeakBar;
						PrevPeakBarsAgo = (CurrentBar - PrevPeakBar);
					
						LastPeaksmi = smi[1];
						LastPeakPrice = High[1];
						LastPeakBar = CurrentBar - 1;
						LastPeakBarsAgo = 1;
					
				//	DrawVerticalLine("smi_Peak"+CurrentBar,1,Color.Chartreuse,DashStyle.Dash,1);  // Draws a vertical line on Indicator at Peaks
					
//						Print(" ");
//						Print("----------------------------------- ");
//						Print(Time[0]);
//						Print("CurrentBar= "+ CurrentBar );
//						Print("Peak in smi ");
//						Print("LastPeakPrice= "+LastPeakPrice);
//						Print("PrevPeakPrice="+ PrevPeakPrice);
//						Print("ThirdPeakPrice="+ ThirdPeakPrice);
//						Print("LastPeaksmi= "+LastPeaksmi);
//						Print("PrevPeaksmi="+ PrevPeaksmi);
//						Print("ThirdPeaksmi="+ ThirdPeaksmi);
//						Print("Bars to PrevPeak="+ PrevPeakBarsAgo);
//						Print("Bars to ThirdPeak="+ ThirdPeakBarsAgo);
					

					// Check for regular divergence in Peaks on  smi line

						if( LastPeakPrice  >= PrevPeakPrice  && LastPeaksmi <= PrevPeaksmi)
						{	
							DrawLine("PSMI1"+CurrentBar.ToString(),false,PrevPeakBarsAgo,PrevPeakPrice + (SignalOffset*TickSize),1,LastPeakPrice + (SignalOffset*TickSize),Color.OrangeRed,DashStyle.Dot,3);
						}
						else
						{RemoveDrawObject ("PSMI1"+CurrentBar);}
						
						//		DrawLine("PSMI2"+CurrentBar.ToString(),false,PrevPeakBarsAgo,PrevPeaksmi,1,LastPeaksmi,Color.White,DashStyle.Dash,2);

							// Check for 1 to 3 point Divergence
							
						if( LastPeakPrice  >= ThirdPeakPrice  && LastPeaksmi <= ThirdPeaksmi)
						{
							DrawLine("PSMI3"+CurrentBar.ToString(),false,ThirdPeakBarsAgo,ThirdPeakPrice + (SignalOffset*TickSize),1,LastPeakPrice + (SignalOffset*TickSize),Color.OrangeRed,DashStyle.Dash,3);
						}
						else
						{RemoveDrawObject ("PSMI3"+CurrentBar);}
					//			DrawLine("PSMI4"+CurrentBar.ToString(),false,ThirdPeakBarsAgo,ThirdPeaksmi,PrevPeakBarsAgo,PrevPeaksmi,Color.White,DashStyle.Dash,2);	

		//  ONE TO THREE DRAWLINE IS MESSED UP
			// 	SO I CHANGED LastPeakBarsAgo to a "1" like it was in CCIcolorV4Diver

					}
				}  //ends + side of calcs
		/*------------------------------------------------------------------------------------------------------------------
				Determine LAST VALLEY in smi Line, record values for it 
		-------------------------------------------------------------------------------------------------------------------*/

			// Look For Valleys in smi Line

				if(smi[0] > smi[1] && smi[1] < smi[2] )  // two higher b4 valley   //  && smi[2] < smi[3])
				{

			// Only take valleys that are below smi-5

					if( smi[1] < -9.9999)                     // same... putting in 25 to catch larger waves   I'M USING 20				{
					{
						ThirdValsmi = PrevValsmi;
						ThirdValPrice = PrevValPrice;
						ThirdValBar = PrevValBar;
						ThirdValBarsAgo = (CurrentBar - ThirdValBar);
					
						PrevValsmi = LastValsmi;
						PrevValPrice = LastValPrice;
						PrevValBar = LastValBar;
						PrevValBarsAgo = (CurrentBar - PrevValBar);
						
						LastValsmi = smi[1];
						LastValPrice = Low[1];
						LastValBar = CurrentBar - 1;
						LastValBarsAgo = 1;
					
											
				//	DrawVerticalLine("smi_Val"+CurrentBar,1,Color.Red,DashStyle.Dash,1);  // draw a vertical line on Indicator at Valleys
						
				/*	Print("----------------------------------- ");
					Print(Time[0]);
					Print("Valley in smi ");
					Print(" ");
					Print("LastValPrice="+ LastValPrice);
					Print("PrevValPrice="+ PrevValPrice);
					Print("ThirdValPrice="+ ThirdValPrice);
					Print("LastValsmi="+ LastValsmi);
					Print("PrevValsmi="+ PrevValsmi);
					Print("ThirdValsmi="+ ThirdValsmi);
					Print("Bars to Last Valley smi="+ (CurrentBar - LastValBar));
					Print("Bars to Prev Valley smi="+ (CurrentBar - PrevValBar));
					Print("Bars to Third Valley smi="+ (CurrentBar - ThirdValBar));
				*/
	
					// Test for Divergence between LastVal and PrevVal
						if( LastValPrice <= PrevValPrice  && LastValsmi  >= PrevValsmi)
						{
							DrawLine("VSMI1"+CurrentBar.ToString(),false,PrevValBarsAgo,PrevValPrice - (SignalOffset*TickSize),1,LastValPrice - (SignalOffset*TickSize),Color.LimeGreen,DashStyle.Dot,3);
						}
						else
						{RemoveDrawObject ("VSMI1"+CurrentBar);}
				//			DrawLine("VSMI2"+CurrentBar.ToString(),false,PrevValBarsAgo,PrevValsmi,1,LastValsmi,Color.White,DashStyle.Dash,2);

				// Check for 3 Point Divergence in valleys
				
						if( LastValPrice <= ThirdValPrice && LastValsmi  >= ThirdValsmi  )
						{
							DrawLine("VSMI3"+CurrentBar.ToString(),false,ThirdValBarsAgo,ThirdValPrice - (SignalOffset*TickSize),1,LastValPrice - (SignalOffset*TickSize),Color.LimeGreen,DashStyle.Dash,3);
						}
						else
						{RemoveDrawObject ("VSMI3"+CurrentBar);}
						
				//			DrawLine("VSMI4"+CurrentBar.ToString(),false,ThirdValBarsAgo,ThirdValsmi,PrevValBarsAgo,PrevValsmi,Color.White,DashStyle.Dash,2);
				
					}
			
				}	//ends the (-) side of calcs
				//lastBar = CurrentBar;
			}	//ends percentBarComplete counter	
			
		}  //end divergence calcs
			
			





		}	// END OnBarUpdate() calcs






        #region Properties
// Plots data
			
        [Browsable(false)]			// prevents data series display in indicator properties dialog
        [XmlIgnore()]				// ensures the indicator can be saved/recovered in chart template
        public DataSeries smi                  // i guess this NEEDS TO BE LOWER CASE to not call the SMI indicator in a loop?
        {										
            get { return Values[0]; }
        }

		
        [Browsable(false)]	
        [XmlIgnore()]		
        public DataSeries AvgSMI
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        [XmlIgnore()]	
        public DataSeries ZeroLine
        {
            get { return Values[2]; }			//  zero signal line (both BB outside KC)
        }
		
		
        [Browsable(false)]	
        [XmlIgnore()]
        public DataSeries BBbandTop
        {
            get { return Values[3]; }			//  top BB above/below top KC band
        }

		[Browsable(false)]	
        [XmlIgnore()]
        public DataSeries BBbandBottom
        {
            get { return Values[4]; }			//  bottom BB below/above bottom KC band
        }

		[Browsable(false)]	
        [XmlIgnore()]
        public DataSeries KCohcTop
        {
            get { return Values[11]; }			//  price OHC above upper KC band
        }

		[Browsable(false)]	
        [XmlIgnore()]
        public DataSeries KColcBot
        {
            get { return Values[12]; }			//  price OLC below bottom KC band
        }

		[Browsable(false)]	
        [XmlIgnore()]
        public DataSeries CloseAboveMean
        {
            get { return Values[13]; }			//  price Close Above BB mean during pinch
        }

		[Browsable(false)]	
        [XmlIgnore()]
        public DataSeries CloseBelowMean
        {
            get { return Values[14]; }			//  price Close below BB mean during pinch
        }
		
		

/// <summary>
		/// </summary>
		[Description("Option  to plot or not to plot the BBands")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("ShowBB")]
		public bool ShowBB

		{
			get { return showBB; }
			set { showBB = value; }
		}

/// <summary>
		/// </summary>
		[Description("Option  to plot or not to plot the BBands")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("ShowKC")]
		public bool ShowKC

		{
			get { return showKC; }
			set { showKC = value; }
		}


		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries BBLower
		{
			get { return Values[7]; }
		}
		
		/// <summary>
		/// Get the middle value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries BBMiddle
		{
			get { return Values[6]; }
		}

		/// <summary>
		/// Get the upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries BBUpper
		{
			get { return Values[5]; }
		}


		/// <summary>
		/// Gets the lower value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries KCLower
		{
			get { return Values[10]; }
		}
		
		/// <summary>
		/// Get the middle value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries KCMiddle
		{
			get { return Values[9]; }
		}

		/// <summary>
		/// Get the upper value.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries KCUpper
		{
			get { return Values[8]; }
		}



// BBS variables  &&  KC varialbes
		
		[Description("")]
        [GridCategory("Parameters")]
        public int BBperiod
        {
            get { return bBperiod; }
            set { bBperiod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double BBdevi
        {
            get { return bBdevi; }
            set { bBdevi = Math.Max(0.00, value); }
        }

		[Description("")]
        [GridCategory("Parameters")]
        public int SignalOffset
        {
            get { return signalOffset; }
            set { signalOffset = Math.Max(1, value); }
        }
		
        [Description("")]
        [GridCategory("Parameters")]
        public int KCperiod
        {
            get { return kCperiod; }
            set { kCperiod = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public double KCoffset
        {
            get { return kCoffset; }
            set { kCoffset = Math.Max(0.00, value); }
        }

   [Description("")]
        [GridCategory("Parameters")]
        public int KCATRperiod
        {
            get { return kCATRperiod; }
            set { kCATRperiod = Math.Max(1, value); }
        }



// SMI variables
		
		
				/// <summary>
		/// </summary>
		[Description("")]
		[Category("Parameters")]
		public int PercentDLength
		{
			get { return percentDLength; }
			set { percentDLength = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("")]
		[Category("Parameters")]
		public int PercentKLength
		{
			get { return percentKLength; }
			set { percentKLength = Math.Max(1, value); }
		}
		
		
		
// Divergence varibles
		
		[Description("")]
		[Category("Parameters")]
		public double PercentBarComplete
		{
			get { return percentBarComplete; }
			set { percentBarComplete = Math.Min(1, value); }
		}
		
		
		
		#endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private BBSbywesDivLiveTC[] cacheBBSbywesDivLiveTC = null;

        private static BBSbywesDivLiveTC checkBBSbywesDivLiveTC = new BBSbywesDivLiveTC();

        /// <summary>
        /// BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles
        /// </summary>
        /// <returns></returns>
        public BBSbywesDivLiveTC BBSbywesDivLiveTC(double bBdevi, int bBperiod, int kCATRperiod, double kCoffset, int kCperiod, double percentBarComplete, int percentDLength, int percentKLength, bool showBB, bool showKC, int signalOffset)
        {
            return BBSbywesDivLiveTC(Input, bBdevi, bBperiod, kCATRperiod, kCoffset, kCperiod, percentBarComplete, percentDLength, percentKLength, showBB, showKC, signalOffset);
        }

        /// <summary>
        /// BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles
        /// </summary>
        /// <returns></returns>
        public BBSbywesDivLiveTC BBSbywesDivLiveTC(Data.IDataSeries input, double bBdevi, int bBperiod, int kCATRperiod, double kCoffset, int kCperiod, double percentBarComplete, int percentDLength, int percentKLength, bool showBB, bool showKC, int signalOffset)
        {
            if (cacheBBSbywesDivLiveTC != null)
                for (int idx = 0; idx < cacheBBSbywesDivLiveTC.Length; idx++)
                    if (Math.Abs(cacheBBSbywesDivLiveTC[idx].BBdevi - bBdevi) <= double.Epsilon && cacheBBSbywesDivLiveTC[idx].BBperiod == bBperiod && cacheBBSbywesDivLiveTC[idx].KCATRperiod == kCATRperiod && Math.Abs(cacheBBSbywesDivLiveTC[idx].KCoffset - kCoffset) <= double.Epsilon && cacheBBSbywesDivLiveTC[idx].KCperiod == kCperiod && Math.Abs(cacheBBSbywesDivLiveTC[idx].PercentBarComplete - percentBarComplete) <= double.Epsilon && cacheBBSbywesDivLiveTC[idx].PercentDLength == percentDLength && cacheBBSbywesDivLiveTC[idx].PercentKLength == percentKLength && cacheBBSbywesDivLiveTC[idx].ShowBB == showBB && cacheBBSbywesDivLiveTC[idx].ShowKC == showKC && cacheBBSbywesDivLiveTC[idx].SignalOffset == signalOffset && cacheBBSbywesDivLiveTC[idx].EqualsInput(input))
                        return cacheBBSbywesDivLiveTC[idx];

            lock (checkBBSbywesDivLiveTC)
            {
                checkBBSbywesDivLiveTC.BBdevi = bBdevi;
                bBdevi = checkBBSbywesDivLiveTC.BBdevi;
                checkBBSbywesDivLiveTC.BBperiod = bBperiod;
                bBperiod = checkBBSbywesDivLiveTC.BBperiod;
                checkBBSbywesDivLiveTC.KCATRperiod = kCATRperiod;
                kCATRperiod = checkBBSbywesDivLiveTC.KCATRperiod;
                checkBBSbywesDivLiveTC.KCoffset = kCoffset;
                kCoffset = checkBBSbywesDivLiveTC.KCoffset;
                checkBBSbywesDivLiveTC.KCperiod = kCperiod;
                kCperiod = checkBBSbywesDivLiveTC.KCperiod;
                checkBBSbywesDivLiveTC.PercentBarComplete = percentBarComplete;
                percentBarComplete = checkBBSbywesDivLiveTC.PercentBarComplete;
                checkBBSbywesDivLiveTC.PercentDLength = percentDLength;
                percentDLength = checkBBSbywesDivLiveTC.PercentDLength;
                checkBBSbywesDivLiveTC.PercentKLength = percentKLength;
                percentKLength = checkBBSbywesDivLiveTC.PercentKLength;
                checkBBSbywesDivLiveTC.ShowBB = showBB;
                showBB = checkBBSbywesDivLiveTC.ShowBB;
                checkBBSbywesDivLiveTC.ShowKC = showKC;
                showKC = checkBBSbywesDivLiveTC.ShowKC;
                checkBBSbywesDivLiveTC.SignalOffset = signalOffset;
                signalOffset = checkBBSbywesDivLiveTC.SignalOffset;

                if (cacheBBSbywesDivLiveTC != null)
                    for (int idx = 0; idx < cacheBBSbywesDivLiveTC.Length; idx++)
                        if (Math.Abs(cacheBBSbywesDivLiveTC[idx].BBdevi - bBdevi) <= double.Epsilon && cacheBBSbywesDivLiveTC[idx].BBperiod == bBperiod && cacheBBSbywesDivLiveTC[idx].KCATRperiod == kCATRperiod && Math.Abs(cacheBBSbywesDivLiveTC[idx].KCoffset - kCoffset) <= double.Epsilon && cacheBBSbywesDivLiveTC[idx].KCperiod == kCperiod && Math.Abs(cacheBBSbywesDivLiveTC[idx].PercentBarComplete - percentBarComplete) <= double.Epsilon && cacheBBSbywesDivLiveTC[idx].PercentDLength == percentDLength && cacheBBSbywesDivLiveTC[idx].PercentKLength == percentKLength && cacheBBSbywesDivLiveTC[idx].ShowBB == showBB && cacheBBSbywesDivLiveTC[idx].ShowKC == showKC && cacheBBSbywesDivLiveTC[idx].SignalOffset == signalOffset && cacheBBSbywesDivLiveTC[idx].EqualsInput(input))
                            return cacheBBSbywesDivLiveTC[idx];

                BBSbywesDivLiveTC indicator = new BBSbywesDivLiveTC();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BBdevi = bBdevi;
                indicator.BBperiod = bBperiod;
                indicator.KCATRperiod = kCATRperiod;
                indicator.KCoffset = kCoffset;
                indicator.KCperiod = kCperiod;
                indicator.PercentBarComplete = percentBarComplete;
                indicator.PercentDLength = percentDLength;
                indicator.PercentKLength = percentKLength;
                indicator.ShowBB = showBB;
                indicator.ShowKC = showKC;
                indicator.SignalOffset = signalOffset;
                Indicators.Add(indicator);
                indicator.SetUp();

                BBSbywesDivLiveTC[] tmp = new BBSbywesDivLiveTC[cacheBBSbywesDivLiveTC == null ? 1 : cacheBBSbywesDivLiveTC.Length + 1];
                if (cacheBBSbywesDivLiveTC != null)
                    cacheBBSbywesDivLiveTC.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheBBSbywesDivLiveTC = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BBSbywesDivLiveTC BBSbywesDivLiveTC(double bBdevi, int bBperiod, int kCATRperiod, double kCoffset, int kCperiod, double percentBarComplete, int percentDLength, int percentKLength, bool showBB, bool showKC, int signalOffset)
        {
            return _indicator.BBSbywesDivLiveTC(Input, bBdevi, bBperiod, kCATRperiod, kCoffset, kCperiod, percentBarComplete, percentDLength, percentKLength, showBB, showKC, signalOffset);
        }

        /// <summary>
        /// BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles
        /// </summary>
        /// <returns></returns>
        public Indicator.BBSbywesDivLiveTC BBSbywesDivLiveTC(Data.IDataSeries input, double bBdevi, int bBperiod, int kCATRperiod, double kCoffset, int kCperiod, double percentBarComplete, int percentDLength, int percentKLength, bool showBB, bool showKC, int signalOffset)
        {
            return _indicator.BBSbywesDivLiveTC(input, bBdevi, bBperiod, kCATRperiod, kCoffset, kCperiod, percentBarComplete, percentDLength, percentKLength, showBB, showKC, signalOffset);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.BBSbywesDivLiveTC BBSbywesDivLiveTC(double bBdevi, int bBperiod, int kCATRperiod, double kCoffset, int kCperiod, double percentBarComplete, int percentDLength, int percentKLength, bool showBB, bool showKC, int signalOffset)
        {
            return _indicator.BBSbywesDivLiveTC(Input, bBdevi, bBperiod, kCATRperiod, kCoffset, kCperiod, percentBarComplete, percentDLength, percentKLength, showBB, showKC, signalOffset);
        }

        /// <summary>
        /// BBSbywesDiver is a BBsqueeze  indicator by Wes Pittman. I took John Carter's idea and coded this little ditty with a few extra bells and whistles
        /// </summary>
        /// <returns></returns>
        public Indicator.BBSbywesDivLiveTC BBSbywesDivLiveTC(Data.IDataSeries input, double bBdevi, int bBperiod, int kCATRperiod, double kCoffset, int kCperiod, double percentBarComplete, int percentDLength, int percentKLength, bool showBB, bool showKC, int signalOffset)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.BBSbywesDivLiveTC(input, bBdevi, bBperiod, kCATRperiod, kCoffset, kCperiod, percentBarComplete, percentDLength, percentKLength, showBB, showKC, signalOffset);
        }
    }
}
#endregion
