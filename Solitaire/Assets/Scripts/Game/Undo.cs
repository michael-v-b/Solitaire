using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Undo : MonoBehaviour
{
    [SerializeField] Stack<string> undoList = new Stack<string>();
    [SerializeField] GameManager gm = null;
    [SerializeField] Button b;
    [SerializeField] Redo r;
    bool won = false;
    bool changeLocation = true;

    /*
    Possible Moves:

    Click Deck = d

    resetDeck = r

    Move from
        Deck = D
        Foundation = F
        Stacks = S
    Move to
        Foundation = F
        Stacks = S

    Syntax:
        To#From#
        S2S3
    */
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
    void Update() {
        if(undoList.Count <= 0) {
            deactivateButton();
        } else if(!won) {
            reactivateButton();
        }
        
    }

    // Start is called before the first frame update
    public void addString(string s){
        undoList.Push(s);
    }

    public void UndoMove() {
        gm.moves.decrementMoves();
        string move = undoList.Pop();
        r.addString(move);
      
        int split = move.IndexOf(';');
        
        string to = "";
        int y = 0;
        if(move != "d" && move != "r") {
            to = move.Substring(split+1,1);
        }

        string first = move.Substring(0,1);
        int x = 0;
        Vector3 location = Vector3.zero;
        GameObject cardObject = null;
        Card selectedCard = null;
        //end locatio nof previous move
        switch(first) {
            //clicked deck
            case "d":
                cardObject = gm.revealedDeck.Peek();
                selectedCard = cardObject.GetComponent<Card>();
                selectedCard.unflip();
                gm.deck.Push(gm.revealedDeck.Pop());
                if(gm.revealedDeck.Count > 0) {
                    gm.revealedDeck.Peek().GetComponent<Collider2D>().enabled = true;
                    gm.revealedDeck.Peek().transform.position = gm.revealDeckPosition.transform.position-new Vector3(0,0,gm.revealedDeck.Count*.1f);
                }
                
                location = gm.deckPosition.transform.position;
                break;
            //reset deck
            case "r":
                while(gm.deck.Count > 0) {
                    gm.deck.Peek().transform.SetParent(gm.revealDeckPosition.transform);
                    gm.deck.Peek().GetComponent<Movement>().moveCard(gm.deck.Peek().transform,gm.revealDeckPosition.transform.position-new Vector3(0,0,gm.revealedDeck.Count*.1f),.5f);
                    gm.revealedDeck.Push(gm.deck.Pop());
                    gm.revealedDeck.Peek().GetComponent<Card>().flip();
                }

                return;
                break;
                //move card from
            //from Foundation
            case "F":
                x = int.Parse(move.Substring(1,1));
                Foundation found = gm.foundations[x].GetComponent<Foundation>();
                cardObject = found.getCardStack().Peek().gameObject;
                
                selectedCard = cardObject.GetComponent<Card>();
                gm.removeFromFoundation(selectedCard,found );
                
                break;

                //move card from foundation x to location
            //from Stack
            case "S":
                int coordSplit = move.IndexOf('.');
                x = int.Parse(move.Substring(1,coordSplit-1));
                Debug.Log("size of index" + ((move.IndexOf(';') -coordSplit-1)));
                int i = int.Parse(move.Substring(coordSplit+1,(move.IndexOf(';') -coordSplit-1)));
                int tempI = 0;
                Debug.Log("x is " + x + " i is " + i);
                GameObject temp = gm.stacks[x];

                // find card in stack
                while(tempI != i+1) {
                    if(temp.transform.childCount > 0) {
                        temp = temp.transform.GetChild(0).gameObject;
                    }
                    tempI++;
                }


                cardObject = temp.gameObject;
                selectedCard = cardObject.GetComponent<Card>();

                gm.removeFromStack(selectedCard,gm.codeStack[x],x);
                //move card i from stack x to location y
                break;
        }
        
       
        //origin of previous move
        switch(to) {
            case "D":
                
                gm.revealedDeck.Push(cardObject);
                cardObject.transform.SetParent(gm.revealDeckPosition.transform);
                selectedCard.setDeck(true);
                location = gm.revealDeckPosition.transform.position-new Vector3(0,0,gm.revealedDeck.Count*.1f);
                break;
            case "F":
                y = int.Parse(move.Substring(split+2,1));
                Foundation found = gm.foundations[y].GetComponent<Foundation>();
                if(found.getCardStack().Count > 0) {
                    location = found.getCardStack().Peek().gameObject.transform.position-new Vector3(0,0,.1f);
                } else {
                    location = found.gameObject.transform.position-new Vector3(0,0,.1f);
                }
                gm.moveToFoundation(selectedCard, found);
                cardObject.transform.SetParent(found.gameObject.transform);

                break;
            case "S":
                //push all new cards into stack
                y = int.Parse(move.Substring(split+2,1));
                char flipped = move[move.Length-1];
                if(flipped == 'n' && gm.codeStack[y].Count > 0) {
                    gm.codeStack[y].Peek().GetComponent<Card>().unflip();
                }
                gm.moveToStack(selectedCard,gm.codeStack[y],y);

                //if is a stack set location to actual transform
                Vector3 parentTransform = cardObject.transform.parent.position;
                if(cardObject.transform.parent.gameObject.layer == 11){
                    cardObject.transform.parent.GetComponent<Collider2D>().enabled = false;
                    location = parentTransform;
                } else {
                    //location = new Vector3(parentTransform.x, parentTransform.y-gm.getCardSpace(y),parentTransform.z-.1f);
                    changeLocation = false;
                }
                break;
            case "":
                break;
        }
        //move card
        if(changeLocation){
            cardObject.GetComponent<Movement>().moveCard(cardObject.transform,location,.1f);
        } else {
            changeLocation = true;
        }

    }

}
