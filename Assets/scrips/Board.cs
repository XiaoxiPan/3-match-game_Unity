﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour {

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefeb1;
    public GameObject tilePrefeb2;
    private BackgroundTile[,] allTiles;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject destroyEffect;
    private FindMatches findMatches;

    public Dot currentDot;




    // Use this for initialization
    void Start () {
        //Console.Write("5");
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width,height];
        allDots = new GameObject[width, height];
        SetUp();
	}

    private void SetUp(){
        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                Vector2 tempPosition1 = new Vector2(i, j);
                Vector2 tempPosition2 = new Vector2(i, j + offSet);

                GameObject backgroundTile;
                if ((i + j) % 2 == 0)
                {
                     backgroundTile = Instantiate(tilePrefeb1, tempPosition1, Quaternion.identity) as GameObject;
                }
                else {
                     backgroundTile = Instantiate(tilePrefeb2, tempPosition1, Quaternion.identity) as GameObject;
         
                }
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";
                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;
                while (MatchsAt(i, j, dots[dotToUse]))
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition2, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";
                allDots[i, j] = dot;
            }

        }

    }
	
    private bool MatchsAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
                
            }
            if(column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
                

        }

        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if(firstPiece != null)
        {
            foreach(GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if(dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);

    }

    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //make a color bomb
                //is the current dot matched?
                if(currentDot != null)
                {
                    if(currentDot != null)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColorBomb();
                        }
                        else
                        {
                            if(currentDot.otherDot != null)
                            {
                                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                                if (otherDot.isMatched)
                                {
                                    if (!otherDot.isColorBomb)
                                    {
                                        otherDot.isMatched = false;
                                        otherDot.MakeColorBomb();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //make a adjacent bomb
                //is the current dot matched?
                if (currentDot != null)
                {
                    if (currentDot != null)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                        else
                        {
                            if (currentDot.otherDot != null)
                            {
                                Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                                if (otherDot.isMatched)
                                {
                                    if (!otherDot.isAdjacentBomb)
                                    {
                                        otherDot.isMatched = false;
                                        otherDot.MakeAdjacentBomb();
                                    }
                                }
                            }
                        }
                    }
                }
         
            }
        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //how may elements are in the matched pieces list from findmatches?
            if(findMatches.currentMatches.Count >= 4 )
            {
                CheckToMakeBombs();
            }
            //findMatches.currentMatches.Remove(allDots[column, row]);
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column= i;
                }
            }
        }
    }

    private bool MatchsOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (MatchsOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move; 
    }
}
