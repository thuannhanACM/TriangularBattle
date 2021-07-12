using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasualTemplate
{
	/// <summary>
	/// 常駐する(DontDestroyOnLoadを設定する)GameObjectにアタッチすること
	/// </summary>
	public class TenjinSDK : MonoBehaviour
	{
		// Tenjin API Key
		private const string TENJIN_API_KEY = "TELEXEDPZA7CMPVDYRRWUDCZMRFQY6QE";

		// Start is called before the first frame update
		void Start()
		{
			InitializeTenjin();
		}

		void InitializeTenjin()
		{
			BaseTenjin instance = Tenjin.getInstance(TENJIN_API_KEY);
			instance.Connect();
		}
	}
}
