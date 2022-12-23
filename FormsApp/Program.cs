using System.Diagnostics;

namespace FormsApp;

internal static class Program
{
	public static ApplicationStartUp app;

	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();
		Application.Run(new Form1());
	}

	[STAThread]
	public static void Visualization()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		app = new ApplicationStartUp();
		app.BackColor = Color.FromArgb(1, 1, 1);
		//f.FormBorderStyle = FormBorderStyle.None;
		app.Bounds = new Rectangle(1000, 100, 630, 630);
		app.TopMost = true;


		Application.Run(app);
	}
}


public class ApplicationStartUp : Form
{
	private void InitializeComponent()
	{

	}

	//Ctor
	public ApplicationStartUp()
	{
		InitializeComponent();

		//Timer tmr = new Timer();
		//tmr.Interval = 10;   // milliseconds
		//tmr.Tick += Tmr_Tick;  // set handler
		//tmr.Start();

		// Sets g to a graphics object representing the drawing surface of the  
		// control or form g is a member of.  

	}

	public void Redraw()
	{
		using (var g = this.CreateGraphics())
		{
			g.Clear(Color.FromArgb(1, 1, 1));
			//g.DrawRectangle(new Pen(Color.Blue, 20f), new Rectangle(1 * Main.Pixel.x, 50, 50, 50));
			g.DrawLine(new Pen(Color.White, 2f), new Point(0, 0), new Point(100, 0));

			for (var i = 0; i <= 20; i++)
			{
				g.DrawLine(new Pen(Color.White, 2f), new Point(0, 30 * i), new Point(20 * 30, 30 * i));
				g.DrawLine(new Pen(Color.White, 2f), new Point(30 * i, 0), new Point(30 * i, 20 * 30));
			}

			for (var i = 0; i < 20; i++)
			{
				for (var j = 0; j < 20; j++)
				{
					//if (Main.grid[i][j])
					//{
					//	g.FillRectangle(Brushes.Green, new Rectangle(30 * i, 30 * j, 30, 30));
					//}
				}
			}
		}
	}

	private void Tmr_Tick(object sender, EventArgs e)  //run this logic each timer tick
	{
		Redraw();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
	}

	private void OnExit(object sender, EventArgs e)
	{
		Debug.WriteLine("OnExit");
		// Release the icon resource.
		Application.Exit();
	}

	protected override void Dispose(bool isDisposing)
	{
		if (isDisposing)
		{
		}
		base.Dispose(isDisposing);
		Environment.Exit(0);
	}
}