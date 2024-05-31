using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] GameObject revealDeckPosition;
    [SerializeField] GameObject deckPosition;
    [SerializeField] GameManager gm;

    public void deckFlip(Stack<GameObject> revealedDeck,Stack<GameObject> deck, Undo u) {
        gm.moves.incrementMoves();
        if(deck.Count <= 0 && revealedDeck.Count > 0) {
            u.addString("r");
            this.resetDeck(revealedDeck,deck);
            return;
        } else if(deck.Count <= 0 && revealedDeck.Count <= 0){
            return;
        }
        GameObject temp;
        temp = deck.Pop();
        if(revealedDeck.Count > 0) {   
            revealedDeck.Peek().GetComponent<Collider2D>().enabled = false;
            temp.GetComponent<Collider2D>().enabled = true;
        }
        revealedDeck.Push(temp);
        temp.gameObject.transform.position = new Vector3(temp.gameObject.transform.position.x, temp.gameObject.transform.position.y,revealDeckPosition.transform.position.z-revealedDeck.Count*.1f);
        temp.GetComponent<Movement>().moveCard(temp.gameObject.transform,revealDeckPosition.transform.position-new Vector3(0,0,revealedDeck.Count* 0.1f),.5f);
        
        
        Card newCard = revealedDeck.Peek().GetComponent<Card>();
        newCard.flip();
        revealedDeck.Peek().transform.SetParent(revealDeckPosition.transform);
        u.addString("d");
    }

    public void resetDeck(Stack<GameObject> revealedDeck,Stack<GameObject> deck) {
        revealedDeck.Peek().GetComponent<Card>().unflip();
        while(revealedDeck.Count > 0) {
            revealedDeck.Peek().transform.SetParent(deckPosition.transform);
            gm.revealedDeck.Peek().GetComponent<Card>().unflip();
            gm.revealedDeck.Peek().GetComponent<Collider2D>().enabled = false;
            gm.revealedDeck.Peek().GetComponent<Movement>().moveCard(revealedDeck.Peek().transform,deckPosition.transform.position-new Vector3(0,0,.1f),.5f);
            deck.Push(revealedDeck.Pop());
        }
    }
}
