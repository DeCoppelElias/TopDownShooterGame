using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using Dan.Models;

namespace LeaderboardCreatorDemo
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private int maxEntries = 100;

        [SerializeField] private GameObject _entryPrefab;
        [SerializeField] private Transform _contentParent;

        [SerializeField] private Text bestScoreText;
        [SerializeField] private Text rankText;

        private List<Text> _entryTextObjects = new List<Text>();
        [SerializeField] private InputField _usernameInputField;

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject firstSelected = null;
        private string currentControlScheme;
        private int Score
        {
            get
            {
                if (PlayerPrefs.HasKey("HighScore"))
                {
                    return (int)PlayerPrefs.GetFloat("HighScore");
                }
                return 0;
            }
        }

        private void Start()
        {
            // Set best score
            bestScoreText.text = Score.ToString();

            // Load leaderboard
            LoadEntries();
        }

        private void CreateEntries(int amount)
        {
            int currentAmount = _entryTextObjects.Count;
            if (currentAmount < amount)
            {
                for (int i = 0; i < amount - currentAmount; i++)
                {
                    _entryTextObjects.Add(Instantiate(_entryPrefab, _contentParent).GetComponentInChildren<Text>());
                }
            }
            else if (currentAmount > amount)
            {
                for (int i = 0; i < currentAmount - amount; i++)
                {
                    Text currentText = _entryTextObjects[i];
                    _entryTextObjects.RemoveAt(i);
                    Destroy(currentText.gameObject);
                }
            }
        }

        private void LoadEntries()
        {
            Leaderboards.ScoreLeaderboard.GetEntries(entries =>
            {
                foreach (var t in _entryTextObjects)
                    t.text = "";

                var length = Mathf.Min(maxEntries, entries.Length);
                CreateEntries(length);

                bool uploaded = false;
                for (int i = 0; i < length; i++)
                {
                    Entry entry = entries[i];
                    Text entryTextObject = _entryTextObjects[i];
                    entryTextObject.text = $"{entry.Rank}. {entry.Username} - {entry.Score}";

                    if (entry.IsMine())
                    {
                        entryTextObject.GetComponentInParent<Image>().color = Color.white;

                        rankText.text = entry.Rank.ToString();

                        uploaded = true;
                    }
                }

                if (!uploaded)
                {
                    rankText.text = "No rank yet!";
                }
            });
        }

        public void UploadEntry()
        {
            if (_usernameInputField.text == "") return;

            Leaderboards.ScoreLeaderboard.UploadNewEntry(_usernameInputField.text, Score, isSuccessful =>
            {
                if (isSuccessful)
                    LoadEntries();
            });
        }

        public void ToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void OnControlsChanged()
        {
            if (!playerInput.enabled) return;

            if (Gamepad.current != null && Gamepad.current.enabled && playerInput.currentControlScheme == "Gamepad")
            {
                EventSystem.current.SetSelectedGameObject(firstSelected);
                _usernameInputField.interactable = false;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                _usernameInputField.interactable = true;
            }
        }

        private void Update()
        {
            if (playerInput.currentControlScheme != currentControlScheme)
            {
                currentControlScheme = playerInput.currentControlScheme;
                OnControlsChanged();
            }
        }
    }
}
