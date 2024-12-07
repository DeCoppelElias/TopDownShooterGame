using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Leaderboards.ScoreLeaderboard.GetEntries(entries =>
        {
            if (entries.Length != 0 && entries[0].IsMine())
            {
                Color color = this.GetComponent<SpriteRenderer>().color;
                color.a = 255;
                this.GetComponent<SpriteRenderer>().color = color;
            }
        });
    }
}
