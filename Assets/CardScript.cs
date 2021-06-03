using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSpriteRenderer;
    [SerializeField] private SpriteRenderer cardSpriteRenderer;

    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float hidingSpeed = 10f;
    [SerializeField] private float fadingSpeed = 10f;
    [SerializeField] private float hideEffectDuration = 10f;
    [SerializeField] private Vector3 flippedRotation = new Vector3(0f, 180f, 0f);
    [SerializeField] private Vector3 hoverRotation = new Vector3(0f, 0f, 6f);
    [SerializeField] private float hoverScale = 1.1f;

    private Vector2 boardPosition;
    private Vector2 hidingPosition;
    private Vector2 targetPosition;
    private bool isHiding = false;
    private bool isFading = false;
    private bool isCardStateChanged = false;
    private float currentAlphaColour = 1;

    private CardsManager cm;
    private bool isFliped = false;
    private bool isHover = false;
    private Vector3 targetRotation;
    private Vector3 targetScale;
    private bool canInteract;
    private Vector3 originalScale;
    public int ID = -1;
  

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        cm = CardsManager.instanceCM;
        
    }

    
    // Update is called once per frame
    void Update()
    {
        if (cm.currentBoardState == CardsManager.BoardState.playerTurn)
        {
            canInteract = true;
        }
        else {
            canInteract = false;
            isHover = false;
            
        }

       
        if (!isFliped && isHover)
        {
            targetRotation = hoverRotation;
            targetScale = originalScale * hoverScale;

        }
        else if (isFliped && !isHover)
        {
            targetRotation = flippedRotation;
            targetScale = originalScale;
        }
        else {
            targetRotation = Vector3.zero;
            targetScale = originalScale;
        }


        transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.eulerAngles, targetRotation, rotationSpeed * Time.deltaTime));
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, hidingSpeed * Time.deltaTime);

        

        if (isFliped && transform.rotation.y > 0.5) 
        {
            iconSpriteRenderer.sortingOrder = 1;
        }
        else
        {
            iconSpriteRenderer.sortingOrder = -1;
        }

        if (isFading) {
            

            if (!isFliped)
            {
                iconSpriteRenderer.color = new Color(1, 1, 1, 0);   
            }
            else
            {
                currentAlphaColour = iconSpriteRenderer.color.a;
                currentAlphaColour = Mathf.Lerp(currentAlphaColour, 0, fadingSpeed * Time.deltaTime);
                iconSpriteRenderer.color = new Color(1, 1, 1, currentAlphaColour);
            }


            currentAlphaColour = cardSpriteRenderer.color.a;
            currentAlphaColour = Mathf.Lerp(currentAlphaColour, 0, fadingSpeed * Time.deltaTime);
            cardSpriteRenderer.color = new Color(1, 1, 1, currentAlphaColour);

        }
        
    }

    private void checkCardState()
    {
        if (isFading)
        {
            StopCoroutine(CompletlyHideCards());
            StartCoroutine(CompletlyHideCards());

        }
        else {
            StopCoroutine(CompletlyHideCards());



            if (isHiding)
            {
                Debug.Log("set hiding here");
                targetPosition = hidingPosition;
                iconSpriteRenderer.color = new Color(1, 1, 1, 1);
                cardSpriteRenderer.color = new Color(1, 1, 1, 1);
            }
            else
            {
                targetPosition = boardPosition;
                iconSpriteRenderer.color = new Color(1, 1, 1, 1);
                cardSpriteRenderer.color = new Color(1, 1, 1, 1);
            }
        }

        
    }

    private void OnMouseOver()
    {
        if (canInteract) {


            if (!isFliped) {
                
                isHover = true;
            }


            if (Input.GetMouseButtonDown(0) && !isFliped)
            {
                FlipCard();
            }

        }
        
    }

    private void OnMouseExit()
    {
        isHover = false;
    }

    public void FlipCard()
    {

        isFliped = true;
        isHover = false;
        CardsManager.instanceCM.flipCard(ID);

    }

    public void UnflipCard() {
        isFliped = false;
        isHover = false;
        //canInteract = false;
    }

    public void HideCard(bool isHiding = true, bool invertHidingPosition = false) {
        this.isHiding = isHiding;
        Debug.Log("hiding: " + isHiding);
        if (invertHidingPosition) hidingPosition = -hidingPosition;

        checkCardState();


        //Debug.Log(targetPosition);
    }
    public void FadeCard(bool isfade = true, bool isHiding = false, bool invertHidingPosition = false)
    {
        this.isFading = isfade;

        //if (!isFading) { HideCard(isHiding) }

        checkCardState();

    }

    public void SetPosition(bool isHiding = true) {
        if (isHiding) transform.position = hidingPosition;
        else transform.position = boardPosition;
    }




    void InitCard() {
        
        isFliped = false;
        isHover = false;
        transform.rotation = Quaternion.identity;
        targetScale = originalScale;
    }

   
    

    public void InitCard(int ID, Sprite cardIcon, Vector2 boardPosition, Vector2 hidingPosition, bool isHiding = true)
    {
        this.ID = ID;
        iconSpriteRenderer.sprite = cardIcon;
        this.boardPosition = boardPosition;
        this.hidingPosition = hidingPosition;
        this.isHiding = isHiding;
        isFliped = false;
        isHover = false;
        isFading = false;
        transform.rotation = Quaternion.identity;
        targetScale = originalScale;
        iconSpriteRenderer.color = new Color(1, 1, 1, 1);
        cardSpriteRenderer.color = new Color(1, 1, 1, 1);
        checkCardState();
    }

    IEnumerator CompletlyHideCards() {
        yield return new WaitForSeconds(hideEffectDuration);
        if (isFading) {
            transform.position = hidingPosition;
            targetPosition = hidingPosition;
            iconSpriteRenderer.color = new Color(1, 1, 1, 0);
            cardSpriteRenderer.color = new Color(1, 1, 1, 0);
        }
        
    }

   
}
