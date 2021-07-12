using UnityEngine;

namespace HyperCasualTemplate
{
    [CreateAssetMenu(menuName = "HCG/ExplosionData")]
    public class ExplosionData : ScriptableObject
    {
        [SerializeField]
        private ParticleSystem effect = null;

        [SerializeField]
        private int score = 0;

        public ParticleSystem Effect
        {
            get { return effect; }
        }

        public int Score
        {
            get { return score; }
        }
    }
}
