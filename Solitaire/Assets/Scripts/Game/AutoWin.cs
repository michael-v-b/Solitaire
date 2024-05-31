using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoWin : MonoBehaviour
{
    [SerializeField] GameManager gm;
    bool winning = false;


    public bool checkStacks() {
        if(gm.revealedDeck.Count > 0 || gm.deck.Count > 0) {
            return false;
        }
        for(int i = 0; i < gm.stacks.Length; i++) {
            GameObject current = gm.stacks[i];
            while(current.transform.childCount > 0) {
                current = current.transform.GetChild(0).gameObject;
                if(!current.GetComponent<Card>().getFlipped()) {
                    return false;
                }
            }
        }
        return true;
    }

    public bool getWinning() {
        return winning;
    }
    public void setWinning(bool b) {
        winning = b;
    }

    //if all cards in stack and deck are flipped
    public IEnumerator autoWinCor() {
        gm.timer.stopTimer();
        winning = true;
        gm.redo.setWon(true);
        gm.redo.deactivateButton();
        gm.undo.setWon(true);
        gm.undo.deactivateButton();
        bool continueWin = true;
        while(continueWin) {
            for(int i = 0; i < gm.stacks.Length; i++) {
                bool temp = false;
                if(gm.codeStack[i].Count > 0) {
                    Card tempCard = gm.codeStack[i].Peek().GetComponent<Card>();
                    //test tempCard for foundations
                    for(int j = 0; j < gm.foundations.Length; j++) {
                        Foundation tempFound = gm.foundations[j].GetComponent<Foundation>();
                        Stack<Card> foundStack = tempFound.getCardStack();

                        //found a stack with a card in it
                        if(foundStack.Count > 0) {
                            if(foundStack.Peek().getSuit() == tempCard.getSuit() && foundStack.Peek().getID() +1 == tempCard.getID()) {
                                //Debug.Log(tempCard.toString() + " was moved to foundation " + j);
                                tempCard.gameObject.transform.SetParent(foundStack.Peek().gameObject.transform);
                                tempCard.GetComponent<Movement>().moveCard(tempCard.gameObject.transform,foundStack.Peek().gameObject.transform.position- new Vector3(0,0,.1f),.05f);
                                gm.removeFromStack(tempCard,gm.codeStack[i],j);
                                gm.moveToFoundation(tempCard,tempFound);
                                temp = true;
                                gm.moves.incrementMoves();
                                break;
                                
                            }
                        //found a stack without a card in it
                        } else if(tempCard.getID() == 1){
                            tempCard.GetComponent<Movement>().moveCard(tempCard.gameObject.transform,tempFound.gameObject.transform.position- new Vector3(0,0,.1f),.05f);
                            tempCard.gameObject.transform.SetParent(tempFound.gameObject.transform);
                            gm.moveToFoundation(tempCard,tempFound);
                            temp = true;
                            gm.moves.incrementMoves();
                            break;
                        }
                    }
                }

                //breaks if card has been moved
                if(temp) {
                    break;
                }
            }
            yield return new WaitForSeconds(.1f);
            //test if stacks still have cards
            continueWin = false;
            for(int i = 0; i < gm.stacks.Length;i++) {
                if(gm.codeStack[i].Count > 0) {
                    continueWin = true;
                    break;
                }
            }
        }
        Debug.Log("winSequence finished");
        gm.win.winGame();
    }
    public void autoWin() {
        Debug.Log("method runs");
        StartCoroutine(autoWinCor());
    }
}
