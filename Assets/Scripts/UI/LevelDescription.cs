using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDescription : MonoBehaviour {

	string m_title;
	string m_description;
	TextMeshProUGUI m_textTitle;
	TextMeshProUGUI m_textDescription;
	Image m_box;
	float m_displayTime = 0f;
	float m_alpha = 0f;

	const float FADESPEED = 0.05f;

	// Use this for initialization
	void Start () {
		m_box = GetComponent<Image> ();
		m_textTitle = transform.Find ("Title").GetComponent<TextMeshProUGUI> ();
		m_textDescription = transform.Find ("Description").GetComponent<TextMeshProUGUI> ();
		setAlpha (0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (m_displayTime > 0f) { 
			m_displayTime -= Time.deltaTime;
			if (m_alpha <= 1f) {
				m_alpha += FADESPEED;
				setAlpha (m_alpha);
			}
		} else if (m_alpha > 0f) {
			m_alpha -= FADESPEED;
			setAlpha (m_alpha);
		}
	}

	public void SetDescription(string title, string description,float timeDisplayed = 5.0f) {
		m_textTitle.text = title;
		m_textDescription.text = description;
		if (timeDisplayed > 0f)
			Display (timeDisplayed);
	}

	public void Display(float displayTime) {
		m_displayTime = displayTime;
	}

	void setAlpha(float alpha) {
		Color c = m_box.color;
		c = new Color (c.r, c.g, c.b, alpha/2f);
		m_box.color = c;
		Color c2 = new Color (1f,1f,1f, alpha);
		m_textTitle.color = c2;
		m_textDescription.color = c2;
	}
}
