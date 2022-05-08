using System.Management;

namespace IronLaboratory
{
    public static class GetBaseBoardInfo
    {
        public static bool GetSN()
        {
            string BaseBoardSN = "";
            const string currentSN = "N0CV1752MB0048525";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
            ManagementObjectCollection information = searcher.Get();

            foreach (ManagementObject obj in information)
            {
                BaseBoardSN = obj["SerialNumber"].ToString();
            }

            if (BaseBoardSN == currentSN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
