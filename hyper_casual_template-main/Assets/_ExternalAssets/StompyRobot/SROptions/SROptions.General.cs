using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    private const string GeneralCategory = "General";

    [Category(GeneralCategory)]
    [DisplayName("Clear All PlayerPrefs")]
    [Sort(99999)]
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
