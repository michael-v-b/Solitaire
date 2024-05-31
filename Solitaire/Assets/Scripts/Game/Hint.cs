using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [SerializeField] Color highlightColor;
    
    public GameObject testCard(Card selectedCard) {
        GameObject cardObject = selectedCard.gameObject;
        //test all foundations
        for(int i = 0; i < gm.foundations.Length; i++) {
            Foundation tempFoundation = gm.foundations[i].GetComponent<Foundation>();
            Stack<Card> foundStack = tempFoundation.getCardStack();
            //if same suit and 1 above enter stack
            if(foundStack.Count > 0) {
                Card tempCard = foundStack.Peek();
                if(tempCard.getSuit() == selectedCard.getSuit() && tempCard.getID()+1 == selectedCard.getID() && selectedCard.gameObject.transform.childCount == 0) {
                    return tempCard.gameObject;
                }
            //if foundation is empty and selected card is ace add
            } else {
                if(selectedCard.getID() == 1) {
                    return gm.foundations[i];
                }
            }
        }
        //test all stacks
        for(int i = 0; i < gm.codeStack.Length; i++) {
            //if stack isn't empty
            if(gm.codeStack[i].Count > 0) {
                Card tempCard = gm.codeStack[i].Peek().GetComponent<Card>();
                if(tempCard.getID() == selectedCard.getID()+1 && selectedCard.testSuit(selectedCard.getSuit(),tempCard.getSuit())) {
                    return tempCard.gameObject;
                }
            //if stack is empty
            } else if(selectedCard.getID() == 13) {
                return gm.stacks[i];
            }
        }
        Debug.Log("return null");
        return null;

    }
    public void getHint() {
        

        GameObject tempCard = null;
        if(gm.revealedDeck.Count > 0) {
            tempCard = testCard(gm.revealedDeck.Peek().GetComponent<Card>());
        }
        if(tempCard) {
            highlightCard(tempCard.GetComponent<Card>());
            highlightCard(gm.revealedDeck.Peek().GetComponent<Card>());
            return;
        } else {
            tempCard = null;
        }
        for(int i = 0; i < gm.codeStack.Length; i++) {
            GameObject currentCard = gm.stacks[i];
            for(int j = 0; j < gm.codeStack[i].Count+1; j++) {
                if(currentCard.transform.childCount > 0) {
                    currentCard = currentCard.transform.GetChild(0).gameObject;
                }
                if(!currentCard.GetComponent<Card>().getFlipped()) {
                    continue;
                }
                tempCard = testCard(currentCard.GetComponent<Card>());
                if(tempCard != null) {
                    highlightCard(tempCard.GetComponent<Card>());
                    highlightCard(currentCard.GetComponent<Card>());
                    return;
                }
            }
        }
    }

    public IEnumerator highlightCardCoroutine(Card card) {
        SpriteRenderer cardSprite = card.gameObject.GetComponent<SpriteRenderer>();
        Color tempColor = cardSprite.color;
        for(int i = 0; i < 4; i++) {
            cardSprite.color = highlightColor;
            yield return new WaitForSeconds(.1f);
            cardSprite.color = tempColor;
            yield return new WaitForSeconds(.1f);
        }
    }
    public void highlightCard(Card card) {
        StartCoroutine(highlightCardCoroutine(card));
        return;
    }
}
