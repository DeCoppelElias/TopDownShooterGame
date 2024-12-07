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

        private List<GameObject> _entryObjects = new List<GameObject>();
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
            int currentAmount = _entryObjects.Count;
            if (currentAmount < amount)
            {
                for (int i = 0; i < amount - currentAmount; i++)
                {
                    _entryObjects.Add(Instantiate(_entryPrefab, _contentParent));
                }
            }
            else if (currentAmount > amount)
            {
                for (int i = 0; i < currentAmount - amount; i++)
                {
                    GameObject currentEntry = _entryObjects[i];
                    _entryObjects.RemoveAt(i);
                    Destroy(currentEntry);
                }
            }
        }

        private void LoadEntries()
        {
            Leaderboards.ScoreLeaderboard.GetEntries(entries =>
            {
                foreach (GameObject entryObject in _entryObjects)
                {
                    entryObject.transform.Find("Name").GetComponent<Text>().text = "";
                    entryObject.transform.Find("Score").GetComponent<Text>().text = "";
                }

                var length = Mathf.Min(maxEntries, entries.Length);
                CreateEntries(length);

                bool uploaded = false;
                for (int i = 0; i < length; i++)
                {
                    Entry entry = entries[i];
                    GameObject entryObject = _entryObjects[i];
                    Text nameText = entryObject.transform.Find("Name").GetComponent<Text>();
                    Text scoreText = entryObject.transform.Find("Score").GetComponent<Text>();
                    GameObject crown = entryObject.transform.Find("Crown").gameObject;

                    nameText.text = $"{entry.Rank}. {entry.Username}";
                    scoreText.text = $"{entry.Score}";
                    if (entry.Rank != 1) crown.SetActive(false);

                    if (entry.IsMine())
                    {
                        entryObject.GetComponent<Image>().color = Color.white;

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
            if (_usernameInputField.text == "" || Score == 0) return;

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
