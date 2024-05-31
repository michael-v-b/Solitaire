using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Redo : MonoBehaviour
{
    
    [SerializeField] Stack<string> redoList = new Stack<string>();
    [SerializeField] GameManager gm = null;
    [SerializeField] Undo undo = null;
    [SerializeField] Deck d;
    [SerializeField] Button b;
    bool won = false;

    public void addString(string s) {
        redoList.Push(s);
    }

    public void redoMove() {
        string move = redoList.Pop();
        int x = 0;
        int y = 0;
        switch(move) {
            case "r":
                d.resetDeck(gm.revealedDeck,gm.deck);
                undo.addString("r");
                return;
                break;
            case "d":
                d.deckFlip(gm.revealedDeck,gm.deck,undo);
                return;
                break;
        }
        int split = move.IndexOf(';');
        string origin = move.Substring(split+1);
        //Debug.Log("split: " + split);
        string goal = move.Substring(0,split);
        Card tempCard = null;
        //Debug.Log("origin: " + origin + " goal: " + goal);
        string originFront = "" + origin[0];
        string goalFront = "" + goal[0];
        switch(originFront) {
            case "S":
//                Debug.Log("origin is stack");
                int coordSplit = origin.IndexOf(".");
//                Debug.Log("i test" + origin.Substring(coordSplit+1,origin.Length-coordSplit-2));
                x = int.Parse(origin.Substring(1,coordSplit-1));
                int i = int.Parse(origin.Substring(coordSplit+1,origin.Length-coordSplit-2));
                Debug.Log("i: " + i);
                tempCard = null;
                GameObject currentObject = gm.stacks[x];
                int tempI = -1;
                while(tempI != i) {
                    if(currentObject.transform.childCount > 0) {
                        currentObject = currentObject.transform.GetChild(0).gameObject;
                    }
                    tempI++;
                }
                tempCard = currentObject.GetComponent<Card>();
                Debug.Log("tempCard: " + tempCard.toString());
                gm.removeFromStack(tempCard,gm.codeStack[x],x);
                break; 
            case "D":
                if(gm.revealedDeck.Count > 0) {
                    tempCard = gm.revealedDeck.Peek().GetComponent<Card>();
                }
                gm.removeFromDeck(tempCard);
                break;
            case "F":
                x = int.Parse(""+origin[1]);
                Foundation tempFound = gm.foundations[x].GetComponent<Foundation>();
                tempCard = tempFound.getCardStack().Peek();
                gm.removeFromFoundation(tempCard,tempFound);
                break;
        }

        switch (goalFront) {
            case "S":
                int coordSplit = goal.IndexOf(".");
                y = int.Parse(goal.Substring(1,coordSplit-1));
                //Debug.Log("y: " + y);
                Stack<GameObject> tempStack = gm.codeStack[y];
                gm.moveToStack(tempCard,tempStack,y);
                break;
            case "F":
                y = int.Parse("" + goal[1]);
                Foundation tempFound = gm.foundations[y].GetComponent<Foundation>();
                GameObject topCard;
                if(tempFound.getCardStack().Count > 0) {
                    topCard = tempFound.getCardStack().Peek().gameObject;
                } else {
                    topCard = tempFound.gameObject;
                }
                gm.moveToFoundation(tempCard,tempFound);
                
                tempCard.gameObject.GetComponent<Movement>().moveCard(tempCard.transform,topCard.transform.position-new Vector3(0,0,.1f),.1f);
                tempCard.transform.SetParent(tempFound.gameObject.transform);
                break;

        }
        gm.moves.incrementMoves();
        undo.addString(move);

    }
    public void clearRedo() {
        int temp = redoList.Count;
        for(int i = 0; i < temp; i++) {
            redoList.Pop();
        }
    }
    public void setWon(bool w) {
        won = w;
    }
    public void deactivateButton() {
        b.enabled = false;
        b.gameObject.GetComponent<Image>().color = new Color(.5f, .5f, .5f);
    }
    public void reactivateButton() {
        b.enabled = true;
        b.gameObject.GetComponent<Image>().color = Color.white;
    }

    // Update is called once per frame
    void Update() {
        if(redoList.Count <= 0) {
            deactivateButton();
        } else if(!won) {
            reactivateButton();
        }
        
    }
}
