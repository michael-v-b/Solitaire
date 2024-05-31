using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foundation : MonoBehaviour
{

    Stack<Card> cardStack = new Stack<Card>();


    public Stack<Card> getCardStack() {
        return cardStack;
    }
    public void add(Card c) {
        
        if(cardStack.Count == 0 ) {
            this.gameObject.GetComponent<Collider2D>().enabled = false;
        }
        
        if(cardStack.Count > 0 && cardStack.Peek().GetComponent<Collider2D>()){
            cardStack.Peek().GetComponent<Collider2D>().enabled = false;
        }
        cardStack.Push(c);
    }

    public void remove(Card c) {
        if(cardStack.Count > 0) {
            cardStack.Pop();
        }
        
        if(cardStack.Count > 0 && cardStack.Peek().GetComponent<Collider2D>()) {
            cardStack.Peek().GetComponent<Collider2D>().enabled = true;
        } else if(cardStack.Count == 0) {
            this.gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }


}
