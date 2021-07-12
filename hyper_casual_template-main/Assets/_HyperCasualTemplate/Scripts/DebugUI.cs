using System.Collections;
using System.Collections.Generic;
using HyperCasualTemplate;
#if !DISABLE_SRDEBUGGER
using SRDebugger;
#endif
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public void Open()
    {
        ConfigData.OpenSRDebuggerWindow();
    }
}
