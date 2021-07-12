using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HyperCasualTemplate
{
    public class TouchExplosion : MonoBehaviour
    {
        [SerializeField]
        private TouchEvent touchEvent = null;

        [SerializeField]
        private List<ExplosionData> explosions = new List<ExplosionData>();

        [SerializeField]
        private Text scoreText = null;

        private int score = 0;
        private Coroutine addScoreCoroutine = null;

        private void Start()
        {
            touchEvent.OnTouchInfo += SetExplosion;
        }

        /// <summary>
        /// タッチして爆発を起こす
        /// </summary>
        /// <param name="info">入力イベント</param>
        private void SetExplosion(Enums.TouchInfo info)
        {
            if (info != Enums.TouchInfo.Began)
            {
                return;
            }

            var explosion = explosions[Random.Range(0, explosions.Count)];

            Vector3 pos = touchEvent.GetTouchPos();
            pos.z -= Camera.main.transform.position.z;
            pos = Camera.main.ScreenToWorldPoint(pos);

            ParticleSystem effect = Instantiate(explosion.Effect);
            effect.transform.position = pos;
            StartCoroutine(_DropExplosion(effect));

            score += explosion.Score;
            if (addScoreCoroutine != null)
            {
                StopCoroutine(addScoreCoroutine);
            }
            addScoreCoroutine = StartCoroutine(_AddScore());
        }

        /// <summary>
        /// 発生したエフェクトを自動で破棄する
        /// </summary>
        /// <param name="effect">対象のエフェクト</param>
        /// <returns>コルーチン処理</returns>
        private IEnumerator _DropExplosion(ParticleSystem effect)
        {
            while (effect.isPlaying)
            {
                yield return null;
            }

            Destroy(effect.gameObject);
        }

        /// <summary>
        /// スコア加算処理
        /// </summary>
        /// <returns>コルーチン処理</returns>
        private IEnumerator _AddScore()
        {
            int start;
            if (!int.TryParse(scoreText.text, out start))
            {
                start = 0;
            }

            int end = score;

            float timer = 0f;
            float duration = 0.2f;

            while (timer <= duration)
            {
                float setScore = Mathf.Lerp(start, end, timer / duration);
                scoreText.text = Mathf.Round(setScore).ToString();
                timer += Time.deltaTime;
                yield return null;
            }

            scoreText.text = score.ToString();
        }
    }
}
