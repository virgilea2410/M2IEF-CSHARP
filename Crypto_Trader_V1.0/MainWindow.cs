using System;
using Gtk;


public partial class MainWindow : Gtk.Window//GtkSharp.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        //Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Gtk.Application.Quit();
        a.RetVal = true;
    }

    protected virtual void Build()
    {
        ////global::Stetic.Gui.Initialize(this);
        ////Widget MainWindow
        //this.Name = "MainWindow";
        ////this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
        //this.WindowPosition = ((global::Gtk.WindowPosition)(4));
        //if ((this.Child != null))
        //{
        //    this.Child.ShowAll();
        //}
        //this.DefaultWidth = 400;
        //this.DefaultHeight = 300;
        //this.Show();
        //this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
    }
}
