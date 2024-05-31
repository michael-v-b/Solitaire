using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   

    //card prefab for initial creation of deck
    [SerializeField] GameObject defaultCard;
    //deck location
    [SerializeField]public GameObject deckPosition;
    //revealDeck position
    [SerializeField] public GameObject revealDeckPosition;
    [SerializeField] StartGame startGame;
    //array of sprites to assign to cards when flipped
    [SerializeField] public Sprite[] cardSprite;
    //location of each stack
    [SerializeField] public GameObject[] stacks = new GameObject[7];
    //location of the 4 foundation piles
    [SerializeField] public GameObject[] foundations = new GameObject[4];
    //array that has card objects
    [SerializeField] public GameObject[] totalCards = new GameObject[52];
    //stack in deck
    [SerializeField] public Stack<GameObject> deck = new Stack<GameObject>();
    //stack in selected from deck
    [SerializeField] public Stack<GameObject> revealedDeck = new Stack<GameObject>();
    //array of stack data structures
    [SerializeField] public Stack<GameObject>[] codeStack = new Stack<GameObject>[7];
    //layer that card objects reside in
    [SerializeField] LayerMask card;
    //layer deckLayer
    [SerializeField] LayerMask deckLayer;
    [SerializeField] bool hasSeed = true;
    public bool paused = false;
    [SerializeField] int debugSeed = 0;
    [SerializeField] static int seed = -1;
    [SerializeField] public Undo undo = null;
    [SerializeField] public Redo redo = null;
    [SerializeField] Hint hint;
    [SerializeField] public Win win;
    [SerializeField] AutoWin aw;
    [SerializeField] public Timer timer;
    [SerializeField] public Moves moves;
    Card hitCard = null;
    string lastMove = "";
    //card that is currently selected
    Card selectedCard;
    float[] cardSpaces = {0.5f,.5f,.5f,.5f,.5f,.5f,.5f};
    bool mouseButtonDown = false;
    bool tempWin = false;
    float timeHeld = 0;

    //moves card from location s, to location f in the alloted amount of frames
 


    public void newSeed() {
        seed = Random.Range(0,int.MaxValue);
    }
    public void restart() {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }
    public void newGame() {
        aw.setWinning(false);
        newSeed();
        restart();
    }
    public void Pause() {

    }

    //randomizes the order of an array
    public void shuffle(GameObject[] t) {
        
        if(hasSeed) {
            Random.seed = debugSeed;
        } else {
            Random.seed = seed;
        }
        for(int i = 0; i < t.Length;i++) {
            int otherCard = Random.Range(i,52);
            GameObject temp;
            temp = t[i];
            t[i] = t[otherCard];
            t[otherCard] = temp;            
        }

    }

    public void moveToFoundation(Card selectedCard, Foundation tempFound){
        selectedCard.setFoundation(tempFound);
        selectedCard.setStackID(-1);
        tempFound.add(selectedCard);

    }
    public void moveToStack(Card selectedCard, Stack<GameObject> currentStack, int currentStackID) {
        if(currentStack.Count > 0) {
            selectedCard.gameObject.transform.SetParent(currentStack.Peek().transform);
        } else {
            selectedCard.gameObject.transform.SetParent(stacks[currentStackID].transform);
        }
        
        Transform iterCard = selectedCard.gameObject.transform;
            if(codeStack[currentStackID].Count <= 0) {
                stacks[currentStackID].gameObject.GetComponent<Collider2D>().enabled = false;
            }
            while(true) {
                currentStack.Push(iterCard.gameObject);
                iterCard.gameObject.GetComponent<Card>().setStackID(currentStackID);
                if(iterCard.childCount > 0) {
                    iterCard = iterCard.GetChild(0);
                } else {
                    break;
                }
            }
        readjustStacks(currentStackID);

    }

    
    //general remove stack method
    public void removeFromStack(Card selectedCard, Stack<GameObject> prevStack, int stackID) {
        Transform iterCard = selectedCard.gameObject.transform;
                        //pop every card in chain
        while(true && prevStack.Count > 0) {
            prevStack.Pop();
            if(iterCard.childCount > 0) {
                iterCard = iterCard.GetChild(0);
            } else {
                break;
            }
        }

        if(prevStack.Count > 0) {
            prevStack.Peek().GetComponent<Card>().flip();
        } else {
            stacks[stackID].GetComponent<Collider2D>().enabled = true;
        }
        readjustStacks(stackID);

    }

    //creates strings for undo
    public string removeFromStack(Card selectedCard, Stack<GameObject> prevStack, int stackID, string lastMove) {
        Transform iterCard = selectedCard.gameObject.transform;
                        //pop every card in chain
        while(true && prevStack.Count > 0) {
            prevStack.Pop();
            if(iterCard.childCount > 0) {
                iterCard = iterCard.GetChild(0);
            } else {
                break;
            }
        }
                        
        if(prevStack.Count> 0 && prevStack.Peek().GetComponent<Card>().getFlipped()){
            lastMove = lastMove +"f";
        } else {
            lastMove = lastMove + "n";
        }
        if(prevStack.Count > 0) {
            prevStack.Peek().GetComponent<Card>().flip();
        } else {
            stacks[stackID].GetComponent<Collider2D>().enabled = true;
        }
        readjustStacks(stackID);
        return lastMove;

    }
    public void removeFromFoundation(Card selectedCard,Foundation found) {
        found.remove(selectedCard);
        selectedCard.resetFoundation();

    }
    public void removeFromDeck(Card selectedCard) {
        selectedCard.setDeck(false);
        if(revealedDeck.Count > 0) {
            revealedDeck.Pop();
        }
        if(revealedDeck.Count > 0) {
            revealedDeck.Peek().GetComponent<Collider2D>().enabled = true;
            //revealedDeck.Peek().transform.position = revealDeckPosition.transform.position-new Vector3(0,0,.1f);
        }
        
    }
    //readjusts stacks after removing a card you don't want selectedCard reallignment
    public void readjustStacks(int stackID) {

        Stack<GameObject> sta = codeStack[stackID];
        List<Vector3> newLocations = new List<Vector3>();
        if(sta.Count <=0) {
            return;
        }

        //change stack adjustment
        if(sta.Count > 7) {
            cardSpaces[stackID] = 3.5f/sta.Count;
        } else {
            cardSpaces[stackID] = 3.5f/7;
        }
        //first card
        GameObject iterCard = stacks[stackID];
        Vector3 origin = iterCard.transform.position;
        //new location
        for(int i = 0; i < sta.Count; i++) {
            newLocations.Add(new Vector3(origin.x, origin.y-cardSpaces[stackID]*i,origin.z-(i+1)*.1f));
        }
        if(iterCard.transform.childCount > 0) {
            iterCard = iterCard.transform.GetChild(0).gameObject;

        }
        
        for(int i = 0; i < newLocations.Count; i++) {
            iterCard.GetComponent<Movement>().moveCard(iterCard.transform,newLocations[i],.1f);
            if(iterCard.transform.childCount> 0) {
                iterCard = iterCard.transform.GetChild(0).gameObject;
            } else {
                return;
            }
        }
    }

    public float getCardSpace(int i) {
        return cardSpaces[i];
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(8,8,false);
        aw.setWinning(false);
        if(seed == -1) {
            newSeed();
        }
        //initializes codeStack array
        for(int i =0; i < codeStack.Length; i++) {
            codeStack[i] = new Stack<GameObject>();
        }

        //makes sure the stacks in the code are initialized before distribution
        startGame.begin(defaultCard);
        //fills remaining 24 cards into deck
        for(int i = 28; i < totalCards.Length; i ++) {
            deck.Push(totalCards[i]);
            totalCards[i].GetComponent<Card>().setDeck(true);
        }
    }

    
    
    // Update is called once per frame
    void Update()

    {
        bool moveHappened = false;
        //raycast for if a card is selected
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //raycast for if the deck is selected
        
        /*if(Input.GetKeyDown(KeyCode.W)) {
            win.debugWin();
        }*/
        

        //select card if right mouse button is pressed
        if(Input.GetMouseButtonDown(0) && !aw.getWinning() && !paused) {
            
            mouseButtonDown = true;
            if(hit.collider != null)
            {
                Debug.Log("mouseButton down");
                timer.startGame();
                //for cards layer
                if(hit.transform.gameObject.layer == 8){
                    selectedCard = hit.collider.gameObject.GetComponent<Card>();
                    if(selectedCard.getFlipped()) {
                        selectedCard.select();
                    }
                }
                //for flip deck layer
                if(hit.transform.gameObject.layer == 9) {
                    hit.transform.gameObject.GetComponent<Deck>().deckFlip(revealedDeck,deck,undo);
                    redo.clearRedo();
                }

            }
        }
        if(mouseButtonDown) {
            timeHeld += Time.deltaTime;
        }
        //if mouse button is released
        if(Input.GetMouseButtonUp(0) && !aw.getWinning() && !paused) {
            mouseButtonDown = false;

            
            //if it hitsomething and currently being seelcted
            if(hit.collider!= null && selectedCard != null) {
                selectedCard.deselect();
                if(timeHeld <=.2f) {
                    if(hint.testCard(selectedCard) != null ){
                        hitCard = hint.testCard(selectedCard).GetComponent<Card>();
                        selectedCard.setHitCard(hitCard);
                    } else {
                        hitCard = null;
                    }
                }

                //tests if the card has been dropped on another card, 
                //if so set parent and move card to respective stack
                if(selectedCard.getHitCard()!=null) {
                    hitCard = selectedCard.getHitCard();
                }

                if(hitCard != null) {
                    Debug.Log("hitCard is not null");
                    moveHappened = true;
                    lastMove = "";
                    int prevStackID = -2;
                    int currentStackID = -2;
                    Stack<GameObject> prevStack = null;
                    Stack<GameObject> currentStack = null;
                    Foundation tempFound = null;

                    selectedCard.gameObject.transform.SetParent(hitCard.gameObject.transform);

                    //location is not Foundation
                    if(hitCard.getFoundation() == null) {
                        if(prevStackID != -1 && selectedCard.getFoundation() == null) {
                            prevStackID = selectedCard.getStackID();
                            prevStack = codeStack[prevStackID]; 
                        }
                        currentStackID = hitCard.getStackID();
                        currentStack = codeStack[currentStackID];
                       
                    //location is from deck to foundation   
                    } else {
                        if(selectedCard.getFoundation() == null && !selectedCard.getDeck()){
                            prevStackID = selectedCard.getStackID();
                            prevStack = codeStack[prevStackID];  
                        }
                        tempFound = hitCard.getFoundation();
                    }

                    
                    

                    //if card is from deck
                    if (selectedCard.getDeck()) {
                        this.removeFromDeck(selectedCard);
                        lastMove = "D";
                    //if card is from foundations
                    } else if (selectedCard.getFoundation()!= null) {
                        
                        lastMove = "F" + selectedCard.getFoundation().gameObject.name;
                        removeFromFoundation(selectedCard,selectedCard.getFoundation());
  
                    //interstack cards
                    } else {
                        lastMove = this.removeFromStack(selectedCard, prevStack, prevStackID,lastMove);
                        lastMove = "S" + prevStackID.ToString() + "." + prevStack.Count.ToString() + lastMove;

                    }

                    lastMove = ";" + lastMove;

                    //if they go to stack
                    if(currentStack != null) {
                        //push every card in chain
                        lastMove = "S" + currentStackID.ToString() + "." + currentStack.Count.ToString() + lastMove;
                        this.moveToStack(selectedCard,currentStack,currentStackID);
                        
                    //if they go to foundation
                    } else if(selectedCard.gameObject.transform.childCount == 0) {
                        lastMove = "F" + tempFound.gameObject.name + lastMove;
                        this.moveToFoundation(selectedCard,tempFound);

                        
                    }
                    moves.incrementMoves();
                    undo.addString(lastMove);
                    redo.clearRedo();
                    selectedCard.resetHitCard();
                    hitCard = null;
                    if(aw.checkStacks()) {
                        aw.autoWin();
                    }
                }
            

                
                    //if hits card do it slightly below card, otherwise same coords
                    //to stack
                Vector3 parentTransform = selectedCard.gameObject.transform.parent.transform.position;
                if(selectedCard.gameObject.transform.parent.gameObject.layer == 8 && selectedCard.gameObject.transform.parent.GetComponent<Card>().getFoundation() == null && !moveHappened) {
                        //spaghetti code
                        //selectedCard.GetComponent<Movement>().moveCard(selectedCard.gameObject.transform, new Vector3(parentTransform.x, parentTransform.y-cardSpaces[selectedCard.getStackID()],parentTransform.z-.1f),.1f);
                    selectedCard.GetComponent<Movement>().moveCard(selectedCard.gameObject.transform, new Vector3(parentTransform.x, parentTransform.y-cardSpaces[selectedCard.getStackID()],parentTransform.z-.1f),.1f);
                        // card is from deck
                } else if(selectedCard.getDeck() && !moveHappened) {

                    selectedCard.GetComponent<Movement>().moveCard(selectedCard.gameObject.transform,revealDeckPosition.transform.position - new Vector3(0,0,revealedDeck.Count*.1f),.1f);

                } else if(selectedCard.getFoundation() != null || selectedCard.gameObject.transform.parent.gameObject.layer == 11){
                    selectedCard.GetComponent<Movement>().moveCard(selectedCard.gameObject.transform,parentTransform-new Vector3(0,0,.1f),.1f);
                    //selectedCard.gameObject.transform.position = parentTransform - new Vector3(0,0,.1f);
                }
                selectedCard = null;
                
                
            }
            timeHeld = 0;
            tempWin = true;
            for(int i = 0; i < foundations.Length; i++) {
                Foundation tempFound = foundations[i].GetComponent<Foundation>();
                if(tempFound.getCardStack().Count < 13) {
                    tempWin = false;
                    break;
                }
            }
            if(tempWin) {

                win.winGame();
            }
        
        }
    }
}
