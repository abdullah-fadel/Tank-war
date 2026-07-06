using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    public class LevelSelectCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text worldNameLabel;
        [SerializeField] private TMP_Text levelNameLabel;
        [SerializeField] private GameObject[] starIcons;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private Button playButton;

        public void Setup(string worldName, string levelName, int stars, bool unlocked, Action onPlay)
        {
            worldNameLabel.text = worldName;
            levelNameLabel.text = levelName;
            lockedOverlay.SetActive(!unlocked);
            playButton.interactable = unlocked;

            for (int i = 0; i < starIcons.Length; i++)
                starIcons[i].SetActive(i < stars);

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => onPlay());
        }
    }
}
