using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] int id;
    [SerializeField] int suit;
    
    [SerializeField] Sprite flippedCard;
    Sprite sprite;
    SpriteRenderer spriteRenderer;
    bool selected = false;
    bool flipped = false;
    bool inDeck = false;
    bool trail = false;
    [SerializeField] Foundation foundation = null;
    GameObject card;
    [SerializeField] GameObject cardSprite;
    Collider2D collider;
    Card hitCard = null;
    [SerializeField] int stackID;
    //used to determine whether cards can be dragged in it


    //suit
    //0 = clubs
    //1 = diamonds
    //2 = hearts
    //3 = spade
   
    public Card () {
        id = 0;
        suit = 0;
    }
    
    public Card(int i, int su, Sprite sp) {
        id = i;
        suit = su;
        sprite = sp;
    }

    //winning animation for trail

    IEnumerator tempTrail() {

        while(trail) {
            yield return new WaitForSeconds(.05f);
            GameObject tempSprite = Instantiate(cardSprite, this.gameObject.transform.position, Quaternion.identity);
            SpriteRenderer spriteComponent = tempSprite.GetComponent<SpriteRenderer>();
            spriteComponent.sortingOrder = spriteRenderer.sortingOrder-1;
            if(spriteComponent.sortingOrder < 2) {
                spriteComponent.sortingOrder = 2;
                spriteRenderer.sortingOrder = 3;
            }
            spriteComponent.sprite = sprite;
        }
    }

    public void startTrail() {
        trail = true;
        StartCoroutine("tempTrail");
    }

    public void OnBecameInvisible() {
        trail = false;
    }



     void Awake() {
        collider = this.gameObject.GetComponent<Collider2D>();
        card = this.gameObject;
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }


    void Update() {
        //sets card's place if dragged
        if(selected) {
            Vector3 tempPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(tempPos.x, tempPos.y,this.transform.position.z);
        }
    }
    
    void OnTriggerStay2D(Collider2D c) {
        Card tempCard = c.gameObject.GetComponent<Card>();
        Debug.Log("happens");
        //normal cards
        if(selected && tempCard != null && tempCard.gameObject.transform.childCount == 0 && tempCard.getFlipped() && tempCard.getID() == id+1) {
            
            if(testSuit(suit,tempCard.getSuit())) {
                hitCard = tempCard;
            }
        //kings
        } else if (selected && id == 13 && tempCard && tempCard.gameObject.layer == 11) {
            //disable stack collider
            hitCard = tempCard;
      
        }

        //test if in foundation
        if(selected && tempCard && tempCard.gameObject.layer == 10){
            Foundation found;
            Stack<Card> foundStack = null;
            if(tempCard.GetComponent<Foundation>()) {
                found = tempCard.GetComponent<Foundation>();
                foundStack = found.getCardStack();
            }
            if(foundStack!= null && foundStack.Count <= 0 && this.getID() == 1) {
                tempCard.setSuit(this.getSuit());
                hitCard = tempCard;
            } 
        }
        if(selected && tempCard && tempCard.getFoundation()!= null && this.getSuit() == tempCard.getSuit() && this.getID() == tempCard.getID()+1){
            hitCard = tempCard;
        }
    }

    void OnTriggerExit2D(Collider2D c) {
        hitCard = null;
    }


    //setter methods
    public void select() {
        bool finishLoop = false;
        selected = true;
        Transform tempGameObject = this.gameObject.transform;
        while(!finishLoop && !trail) {
            tempGameObject.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
            if(tempGameObject.childCount>0) {
                tempGameObject = tempGameObject.GetChild(0);
            } else {
                finishLoop = true;
            }
        }
    }
    public void deselect() {
        bool finishLoop = false;
        selected = false;
        Transform tempGameObject = this.gameObject.transform;
        while(!finishLoop && !trail) {
            tempGameObject.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if(tempGameObject.childCount>0) {
                tempGameObject = tempGameObject.GetChild(0);
            } else {
                finishLoop = true;
            }
        }
    }
    public bool testSuit(int suitA, int suitB) {
        switch(suitA) {
            case 0:
            case 3:
                if(suitB == 1 || suitB ==2) {
                    return true;
                }
                break;
            case 1:
            case 2:
                if(suitB == 0 || suitB == 3) {
                    return true;
                }
                break;
        }
        return false;

    }
    public void flip() {
        collider.enabled = true;
        flipped = true;
        spriteRenderer.sprite = sprite;
    }
    public void unflip() {
        collider.enabled = false;
        flipped = false;
        spriteRenderer.sprite = flippedCard;
    }
    public void resetHitCard() {
        hitCard = null;
    }
    public void setID(int i) {
        this.id = i;
    }

    public void setSuit(int su) {
        this.suit = su;
    }
    public void setSprite(Sprite sp) {
        this.sprite = sp;
    }
    //sets all in stack
    public void setStackID(int i) {
        stackID = i;

    }
    public void setDeck(bool i) {
        inDeck = i;
    }
    public void setFoundation(Foundation f) {
        foundation = f;
    }
    public void setHitCard(Card c) {
        this.hitCard = c;
    }
    public void resetFoundation(){
        foundation = null;
    }
    //getter methods
    public bool getFlipped() {
        return flipped;
    }
    public int getID() {
        return this.id;
    }
    public int getSuit() {
        return this.suit;
    }
    public Sprite getSprite() {
        return this.sprite;
    }
    public Card getHitCard() {
        return hitCard;
    }
    public int getStackID() {
        return stackID;
    }

    public bool getDeck() {
        return inDeck;
    }
    public Foundation getFoundation() {
        return foundation;
    }


    public string toString() {
        string idString = "";
        string suitString = "";
        //set card id
        switch(this.id) {
            case 11:
                idString = "Jack";
                break;
            case 12:
                idString = "Queen";
                break;
            case 13:
                idString = "King";
                break;
            default:
                idString = "" + this.id;
                break;
        }
        //set card suit
        switch(this.suit){
            case 0:
                suitString = "clubs";
                break;
            case 1:
                suitString = "diamonds";
                break;
            case 2:
                suitString = "hearts";
                break;
            case 3:
                suitString = "spades";
                break;
        }
        return idString + " of " + suitString;
    }
}
