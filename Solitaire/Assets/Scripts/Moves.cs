using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Moves : MonoBehaviour
{
    [SerializeField] int moves = 0;
    [SerializeField] TextMeshProUGUI text;
    // Start is called before the first frame update
    public void incrementMoves() {
        moves++;
        this.updateMoves();

    }
    public void decrementMoves() {
        if(moves >= 0) {
            moves--;
        }
        this.updateMoves();
    }
    void updateMoves() {
        text.SetText("Moves: " + moves);
    }
}
