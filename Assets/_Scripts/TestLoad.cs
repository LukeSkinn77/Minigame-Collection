using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization; 
using System.IO;
using System.Xml.Linq;


public class TestLoad : MonoBehaviour
{
    XDocument xDoc;
    IEnumerable<XElement> items;
    List<XMLData> data = new List<XMLData>();

    int iteration = 0, pageNum = 0;

    string charText, dialogueText;

    public bool finishedLoading = false;

    void Start()
    {

        DontDestroyOnLoad(gameObject); //Allows Loader to carry over into new scene LoadXML (); //Loads XML File. Code below. 
        LoadXML();
        StartCoroutine("AssignData"); 
    }

    void Update()
    {
        //if (finishedLoading)
        //{
        //    //Application.LoadLevel(“TestScene”); //Only happens if coroutine is finished 
        //    finishedLoading = false;
        //}
    }

    void LoadXML()
    {
        //Assigning Xdocument xmlDoc. Loads the xml file from the file path listed. 
        xDoc = XDocument.Load("Assets/Resources/Documents/testxml.xml");
        //This basically breaks down the XML Document into XML Elements. Used later. 
        items = xDoc.Descendants("page").Elements ();
    }

    //this is our coroutine that will actually read and assign the XML data to our List 
    IEnumerator AssignData()
    {
        foreach (var item in items)
        {
            if (item.Parent.Attribute("number").Value == iteration.ToString())
            {
                pageNum = int.Parse(item.Parent.Attribute("number").Value);
                charText = item.Parent.Element("name").Value.Trim();
                dialogueText = item.Parent.Element("dialogue").Value.Trim();

                data.Add(new XMLData(pageNum, charText, dialogueText));

                Debug.Log(data[iteration].dialogueText);

                iteration++; 
            }
        }

        finishedLoading = true;
        yield return null;

    }

}

public class XMLData
{
    public int pageNum;
    public string charText, dialogueText;
    public XMLData(int page, string character, string dialogue)
    {
        pageNum = page;
        charText = character;
        dialogueText = dialogue;
    }
}
