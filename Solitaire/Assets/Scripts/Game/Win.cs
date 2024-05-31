using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] GameManager gm;
    SpriteRenderer sprite;
    // Start is called before the first frame update
    IEnumerator winCoroutine() {
        int orderInLayer = 3;
        for(int i = 0; i < 13; i++) {
            for(int j = 0; j < gm.foundations.Length;j ++) {
                Foundation tempFound = gm.foundations[j].GetComponent<Foundation>();
                GameObject tempObject = null;
                //get card at top of foundation
                if(tempFound != null && tempFound.getCardStack().Count > 0) { 
                    tempObject = tempFound.getCardStack().Pop().gameObject;
                }
                //set sprite sorting order
                sprite = tempObject.GetComponent<SpriteRenderer>();
                sprite.sortingOrder = orderInLayer;
                //enable gravity
                Collider2D cardCollider = tempObject.GetComponent<Collider2D>();
                cardCollider.enabled = true;
                cardCollider.isTrigger = false;
                Physics2D.IgnoreLayerCollision(8,8);
                Fall fall = tempObject.GetComponent<Fall>();
                if(fall != null) {
                    fall.setFalling(true);
                    fall.setDirection(Random.Range(-2f,2f));
                    fall.setSpeed(5f);
                }
                tempObject.GetComponent<Card>().startTrail();
                yield return new WaitForSeconds(2);
                orderInLayer++;
            }

        }
    }
    public void winGame() {
        gm.redo.setWon(true);
        gm.redo.deactivateButton();
        gm.undo.setWon(true);
        gm.undo.deactivateButton();
        StartCoroutine("winCoroutine");
    }

    //I don't wanna sort fir
    public void debugWin() {
        bool[] visited = new bool[52];
        for(int i = 0; i < visited.Length; i++) {
            visited[i] = false;
        }
        int currentCard = 1;
        int tempNum = 0;
        for(int i = 0; i < 13; i++) {
            for(int j = 0; j< gm.totalCards.Length; j++) {
                Card tempCard = gm.totalCards[j].GetComponent<Card>();

                if(tempCard != null && tempCard.getID() == currentCard && !visited[j]) {
                    tempCard.flip();
                    //add to foundation
                    GameObject foundObject = gm.foundations[tempCard.getSuit()];
                    Foundation found = foundObject.GetComponent<Foundation>();
                    if(found.getCardStack().Count > 0) {
                        tempCard.gameObject.transform.SetParent(found.getCardStack().Peek().gameObject.transform);
                    } else {
                        tempCard.gameObject.transform.SetParent(foundObject.transform);
                    }
                    found.add(tempCard);
                    tempCard.gameObject.GetComponent<Movement>().moveCard(tempCard.gameObject.transform,foundObject.transform.position + new Vector3(0, 0, -.1f * currentCard),.1f);
                    //set visited
                    visited[j] = true;
                    //increase num
                    tempNum ++;
                    if(tempNum ==4) {
                        tempNum = 0;
                        currentCard++;
                    }
                }

            }
        }
        this.winGame();
    }
}
