using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [SerializeField] GameObject deckPosition;
    // Start is called before the first frame update
   public void begin(GameObject defaultCard) {

    //initializes deck
    this.initializeDeck(defaultCard);
    gm.shuffle(gm.totalCards);
    this.distributeCards();
            

   }

   void initializeDeck(GameObject defaultCard) {
        for(int i = 0; i < 13; i++) {
            for (int j = 0; j < 4; j++) {
                int cardId = i*4+j;
                gm.totalCards[cardId] = Instantiate(defaultCard,deckPosition.transform);
                
                Card thisCard = gm.totalCards[cardId].GetComponent<Card>();
                thisCard.gameObject.GetComponent<Collider2D>().isTrigger = true;
                thisCard.setID(i+1);
                thisCard.setSuit(j);
                thisCard.setSprite(gm.cardSprite[cardId+1]);
            }
        }
   }

   void distributeCards() {
        int cardID= 0;
        for(int i = 0; i < 7; i++) {
            for(int j = i; j < 7; j++) {
                Card tempCard = gm.totalCards[cardID].GetComponent<Card>();
                //top card in deck will be flipped
                if(i == j) {
                    tempCard.flip();
                }
                //if first in stack data structure, set parent to stack location
                if(gm.codeStack[j].Count == 0) {
                    gm.totalCards[cardID].transform.SetParent(gm.stacks[j].transform);
                } else {
                    gm.totalCards[cardID].transform.SetParent(gm.codeStack[j].Peek().transform);
                }
                tempCard.setStackID(j);
                
                gm.codeStack[j].Push(gm.totalCards[cardID]);
                Vector3 tempPosition = gm.stacks[j].transform.position;
                gm.totalCards[cardID].GetComponent<Movement>().moveCard(gm.totalCards[cardID].transform,new Vector3(tempPosition.x, tempPosition.y+(-.5f* i),tempPosition.z+(i*-.01f)),.75f);
                cardID++;
            }
        }
   }


}
