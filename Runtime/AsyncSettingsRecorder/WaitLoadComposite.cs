using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OmiyaGames.Saves
{
	public class WaitLoadComposite : WaitLoad
	{
		public override event LoadingFinished OnLoadingFinished;

		public WaitLoadComposite(params WaitLoad[] waits)
		{
			// FIXME: implement
			throw new System.NotImplementedException();
		}

		// FIXME: implement
		public override bool keepWaiting => throw new System.NotImplementedException();
	}
}
