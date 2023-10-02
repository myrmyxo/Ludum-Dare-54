using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable] public class OneLevel
{
    public string name;
    public Vector2 playerStartPosition;
    public int startingSpaces;
    public GameObject parentGameObject;
    public GameObject[] entitiesList;
}
public class ControllerScript : MonoBehaviour
{
    public Rigidbody2D player;
    public bool facingRight = true;
    public int movementX = 0;
    public int movementY = 0;
    public float speedX = 0;
    public float speedY = 0;

    public bool isPressingSpace;
    public bool isJumping;
    public int spacePressesLeft = 3;

    public float time = 0;
    public float timeAtLastGround = -99;
    public float timeAtLastJump = -99;

    public bool isOnGround;
    public bool isJumpAble;
    public bool isCoyoteJumpAble;
    public bool leftCollision;
    public bool rightCollision;

    public List<string> collectedStuff;

    public List<EntityBehavior> strollerList;

    public List<OneLevel> levelList;
    int levelIndex = 0;

    public Sprite[] playerSprites;
    public Sprite[] strollerSprites;

    public TMPro.TMP_Text spacePressesLeftText;
    public float Sign(float x)
    {
        if (x >= 0) { return 1; }
        return 0;
    }
    public float Abs(float x)
    {
        if (x >= 0) { return x; }
        return -x;
    }
    public void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = (int)movementVector.x;
        movementY = (int)movementVector.y;
    }
    public void OnSpace(InputValue movementValue)
    {
        float value = movementValue.Get<float>();

        if(value > 0) { isPressingSpace = true; }
        else { isPressingSpace = false; }
    }
    void OnChangeLevelToLoad(InputValue movementValue)
    {
        levelIndex = (levelIndex + 1) % levelList.Count;
    }
    void OnLoadDaLevel(InputValue movementValue)
    {
        loadLevel(levelIndex);
    }
    public bool coyoteJumpTest()
    {
        if (!isCoyoteJumpAble) { return false; }
        if(timeAtLastGround < time - 0.08f) { return false; }
        return true;
    }
    public bool groundTest()
    {
        Vector2 playerPosition = player.gameObject.transform.position;
        Vector2 testPositionA;
        Vector2 testPositionB;
        if (facingRight)
        {
            playerPosition.x -= 0.2f;
            playerPosition.y -= 0.4f;
            testPositionA = playerPosition;
            playerPosition.x += 0.4f;
            playerPosition.y -= 0.101f;
            testPositionB = playerPosition;
        }
        else
        {
            playerPosition.x -= 0.2f;
            playerPosition.y -= 0.4f;
            testPositionA = playerPosition;
            playerPosition.x += 0.4f;
            playerPosition.y -= 0.1f;
            testPositionB = playerPosition;
        }

        Collider2D[] collidero = Physics2D.OverlapAreaAll(testPositionA, testPositionB);
        foreach(Collider2D collider in collidero)
        {
            if(collider.gameObject != player.gameObject && (collider.GetComponent<EntityBehavior>() == null || collider.GetComponent<EntityBehavior>().type == 1))
            {
                timeAtLastGround = time;
                isCoyoteJumpAble = true;
                return true; 
            }
        }
        return false;
    }
    public bool jumpAbleTest()
    {
        Vector2 playerPosition = player.gameObject.transform.position;
        Vector2 testPositionA;
        Vector2 testPositionB;
        if (facingRight)
        {
            playerPosition.x -= 0.4f;
            playerPosition.y -= 0.4f;
            testPositionA = playerPosition;
            playerPosition.x += 0.6f;
            playerPosition.y -= 0.2f;
            testPositionB = playerPosition;
        }
        else
        {
            playerPosition.x -= 0.2f;
            playerPosition.y -= 0.4f;
            testPositionA = playerPosition;
            playerPosition.x += 0.6f;
            playerPosition.y -= 0.2f;
            testPositionB = playerPosition;
        }

        Collider2D[] collidero = Physics2D.OverlapAreaAll(testPositionA, testPositionB);
        foreach (Collider2D collider in collidero)
        {
            if (collider.gameObject != player.gameObject && (collider.GetComponent<EntityBehavior>() == null || collider.GetComponent<EntityBehavior>().type == 1))
            {
                return true;
            }
        }
        return false;
    }
    public bool leftCollisionTest()
    {
        Vector2 playerPosition = player.gameObject.transform.position;
        Vector2 testPositionA;
        Vector2 testPositionB;

        playerPosition.x -= 0.6f;
        playerPosition.y += 0.4f;
        testPositionA = playerPosition;
        playerPosition.x += 0.2f;
        playerPosition.y -= 0.8f;
        testPositionB = playerPosition;

        Collider2D[] collidero = Physics2D.OverlapAreaAll(testPositionA, testPositionB);
        if (collidero.Length > 1) { return true; }
        return false;
    }
    public bool rightCollisionTest()
    {
        Vector2 playerPosition = player.gameObject.transform.position;
        Vector2 testPositionA;
        Vector2 testPositionB;

        playerPosition.x += 0.4f;
        playerPosition.y += 0.4f;
        testPositionA = playerPosition;
        playerPosition.x += 0.2f;
        playerPosition.y -= 0.8f;
        testPositionB = playerPosition;

        Collider2D[] collidero = Physics2D.OverlapAreaAll(testPositionA, testPositionB);
        foreach (Collider2D collider in collidero)
        {
            if (collider.gameObject != player.gameObject && !collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }
    public void collectionTest()
    {
        Vector2 playerPosition = player.gameObject.transform.position;
        Vector2 testPositionA;
        Vector2 testPositionB;

        playerPosition.x += 0.55f;
        playerPosition.y += 0.55f;
        testPositionA = playerPosition;
        playerPosition.x -= 1.1f;
        playerPosition.y -= 1.1f;
        testPositionB = playerPosition;

        Collider2D[] collidero = Physics2D.OverlapAreaAll(testPositionA, testPositionB);
        foreach (Collider2D collider in collidero)
        {
            EntityBehavior EntityBehavioro = collider.gameObject.GetComponent<EntityBehavior>();
            if (EntityBehavioro != null)
            {
                if (EntityBehavioro.type == -1)
                {
                    levelIndex++;
                    loadLevel(levelIndex);
                    return;
                }
                if (EntityBehavioro.type == 0 || EntityBehavioro.type == 3)
                {
                    loadLevel(levelIndex);
                }
                if (EntityBehavioro.type == 2)
                {
                    collectedStuff.Add(EntityBehavioro.name);
                    EntityBehavioro.gameObject.SetActive(false);
                }
                else if(EntityBehavioro.type == 1)
                {
                    foreach(string stringo in collectedStuff)
                    {
                        if (stringo.Length > 0 && EntityBehavioro.name.Length > 0 && stringo[0] == EntityBehavioro.name[0])
                        {
                            EntityBehavioro.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
    void movePlayer()
    {
        if (spacePressesLeft < 0)
        {
            player.gameObject.SetActive(false);
            spacePressesLeft = 0;
        }

        isOnGround = groundTest();
        isJumpAble = jumpAbleTest();
        isCoyoteJumpAble = coyoteJumpTest();
        leftCollision = leftCollisionTest();
        rightCollision = rightCollisionTest();
        collectionTest(); 

        if (leftCollision || rightCollision)
        {
            speedX = player.velocity.x * 0.02f;
            if( speedY <= 0) { speedY = player.velocity.y * 0.02f; }
        }
        else { speedY = player.velocity.y * 0.02f; }


        if (movementX > 0)
        {
            facingRight = true;
            speedX = 0.6f * speedX + 0.1f;
        }
        else if (movementX < 0)
        {
            facingRight = false;
            speedX = 0.6f * speedX - 0.1f;
        }
        else
        {
            speedX = Sign(speedX) * MathF.Max(0, Abs(speedX) * 0.5f - 0.1f);
        }
        if (isOnGround)
        {
            if(isPressingSpace && !isJumping)
            {
                isJumping = true;
                spacePressesLeft--;
            }
            else if(!isPressingSpace) { isJumping = false; }

            if (isJumping && timeAtLastJump < time - 0.1f)
            {
                speedY = 0.5f;
                timeAtLastJump = time;
            }
            else
            {
                speedY = 0f;
            }
        }
        else
        {
            if (isPressingSpace && !isJumping && (isJumpAble || isCoyoteJumpAble) && timeAtLastJump < time - 0.1f)
            {
                isJumping = true;
                spacePressesLeft--;
            }
            else if (!isPressingSpace) { isJumping = false; }

            if (isJumping && (isJumpAble || isCoyoteJumpAble) && timeAtLastJump < time - 0.1f)
            {
                speedY = 0.5f;
                isCoyoteJumpAble = false;
                timeAtLastJump = time;
            }
            else
            {
                if (speedY > 0)
                {
                    if (movementY < 0 || !isJumping)
                    {
                        speedY = MathF.Max(0, speedY - 0.15f);
                    }
                }
                if (movementY < 0)
                {
                    speedY = speedY - 0.1f;
                }
                speedY = speedY - 0.05f;
            }
        }

        player.velocity = new Vector2(speedX * 50, speedY * 50);

        if(speedY > 0) { player.gameObject.GetComponent<SpriteRenderer>().sprite = playerSprites[1]; }
        else if (speedY < 0) { player.gameObject.GetComponent<SpriteRenderer>().sprite = playerSprites[2]; }
        else { player.gameObject.GetComponent<SpriteRenderer>().sprite = playerSprites[0]; }

    }
    void loadLevel(int idx)
    {
        time = 0;
        timeAtLastGround = -60;
        timeAtLastJump = -60;
        collectedStuff = new List<string>();
        strollerList = new List<EntityBehavior>();
        foreach (OneLevel level in levelList)
        {
            level.parentGameObject.SetActive(false);
        }
        OneLevel levelToLoad = levelList[idx];
        levelToLoad.parentGameObject.SetActive(true);
        foreach (GameObject entity in levelToLoad.entitiesList)
        {
            entity.SetActive(true);
            EntityBehavior entityBehavior = entity.GetComponent<EntityBehavior>();
            if(entityBehavior.type == 3)
            {
                strollerList.Add(entityBehavior);
            }
            entity.transform.position = entityBehavior.startingPosition;
        }
        player.transform.position = levelToLoad.playerStartPosition;
        player.velocity = new Vector2(0, 0);
        movementX = 0;
        movementY = 0;
        isPressingSpace = false;
        spacePressesLeft = levelToLoad.startingSpaces;
        player.gameObject.SetActive(true);
    }
    void moveStrollers()
    {
        foreach (EntityBehavior entityBehavior in strollerList)
        {
            SpriteRenderer spriteRenderer = entityBehavior.GetComponent<SpriteRenderer>();
            float timo = time + entityBehavior.timeOffset;
            float time1 = timo % (entityBehavior.timeBeforeTurn);
            float time2 = timo % (entityBehavior.timeBeforeTurn * 2);

            int animTime = ((int)(time*10)) % 6;
            spriteRenderer.sprite = strollerSprites[animTime];

            if (entityBehavior.startingDirection == 1)
            {
                time1 = -time1;
                time2 = -time2;
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            if (time2 == time1)
            {
                entityBehavior.transform.position = entityBehavior.startingPosition + new Vector2(time1, 0);
            }
            else
            {
                float time3 = time2-time1*2;
                entityBehavior.transform.position = entityBehavior.startingPosition + new Vector2(time3, 0);
                spriteRenderer.flipX = !spriteRenderer.flipX;

            }
        }
    }
    void updateTexts()
    {
        spacePressesLeftText.text = "Space Presses Left : " + spacePressesLeft.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        loadLevel(0);
    }

    void FixedUpdate()
    {

        float timeElapsed = Time.deltaTime;
        time += timeElapsed;

        moveStrollers();
        movePlayer();

        updateTexts();

        Physics2D.Simulate(timeElapsed);
    }

    // Update is called once per frame
    void Update()
    {
        if(facingRight)
        {
            player.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            player.GetComponent<SpriteRenderer>().flipX = true;

        }
    }
}
