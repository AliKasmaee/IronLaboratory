using System.Windows;

namespace IronLaboratory
{
    public static class CheckSampleID
    {
        public static void ResetSampleID()
        {
            if (SampleRepository.SampleID == 5001)
            {
                SampleRepository.SampleID = 3999;
                MessageBox.Show("شماره نمونه مجدداً تنظیم شد", "Sample ID Reset", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
            }
        }
    }
}
