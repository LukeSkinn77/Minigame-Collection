using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        protected RectTransform currentPanel;

        public void LevelChange(string levelID)
        {
            GameManager.LoadNewLevel(levelID);
        }

        public void Leave()
        {
            Application.Quit();
        }

        protected IEnumerator AlphaShift(TMP_Text obj, float timeToUse, float startAlpha, float endAlpha)
        {
            float currentTime = 0;
            float alpha = endAlpha - startAlpha;
            while (currentTime < timeToUse)
            {
                currentTime += Time.deltaTime;
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a + ((alpha / timeToUse) * Time.deltaTime));
                yield return null;
            }
        }

        protected IEnumerator AlphaShift(Image obj, float timeToUse, float startAlpha, float endAlpha)
        {
            float currentTime = 0;
            float alpha = endAlpha - startAlpha;
            while (currentTime < timeToUse)
            {
                currentTime += Time.deltaTime;
                obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a + ((alpha / timeToUse) * Time.deltaTime));
                yield return null;
            }
        }
    }
}


