using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace IronLaboratory
{
    /// <summary>
    /// Interaction logic for SingleLabelUC.xaml
    /// </summary>
    public partial class SingleLabelUC : UserControl
    {
        public SingleLabelUC()
        {
            InitializeComponent();
        }
        public SingleLabelUC(string r, string m, string e, string c, string d, int i, string o)
        {
            InitializeComponent();

            if (e == "Co2")
            {
                Experiment.FontFamily = Application.Current.Resources["Verdana"] as FontFamily;
                Experiment.Inlines.Add(new Run("Co"));
                Experiment.Inlines.Add(new Run("2") { BaselineAlignment = BaselineAlignment.Subscript });

                Registration.Text = r;
                Material.Text = m;
                Clock.Text = c;
                OnDate.Text = d;
                SampleId.Text = i.ToString();
                OrderId.Text = o;
            }
            else if (m.Length > 26)
            {
                Material.FontSize = 7.6;
                Experiment.FontSize = 7.6;

                Registration.Text = r;
                Material.Text = m;
                Experiment.Text = e;
                Clock.Text = c;
                OnDate.Text = d;
                SampleId.Text = i.ToString();
                OrderId.Text = o;
            }
            else
            {
                Registration.Text = r;
                Material.Text = m;
                Experiment.Text = e;
                Clock.Text = c;
                OnDate.Text = d;
                SampleId.Text = i.ToString();
                OrderId.Text = o;
            }
        }
    }
}
