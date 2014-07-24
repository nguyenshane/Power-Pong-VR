using UnityEngine;
using System.Collections;

public class Splash_Screen: MonoBehaviour
{
	public float UpTimeSpeed = 1;
	public float WaitingTime = 5;
	public float DownTimeSpeed = 1;
	
	private GUITexture splash;
	
	IEnumerator Start()
	{
		Color c = Color.white;
		c.a = 0;
		splash = (GetComponent(typeof(GUITexture)) as GUITexture);
		splash.color = c;

		// Position the billboard in the center, 
		// but respect the picture aspect ratio
		int textureHeight = guiTexture.texture.height;
		int textureWidth = guiTexture.texture.width;
		int screenHeight = Screen.height;
		int screenWidth = Screen.width;
		
		int screenAspectRatio = (screenWidth / screenHeight);
		int textureAspectRatio = (textureWidth / textureHeight);
		
		int scaledHeight;
		int scaledWidth;
		if (textureAspectRatio <= screenAspectRatio)
		{
			// The scaled size is based on the height
			scaledHeight = screenHeight;
			scaledWidth = (screenHeight * textureAspectRatio);
		}
		else
		{
			// The scaled size is based on the width
			scaledWidth = screenWidth;
			scaledHeight = (scaledWidth / textureAspectRatio);
		}
		float xPosition = screenWidth / 2 - (scaledWidth / 2);
		//splash.pixelInset = 
		//	new Rect(xPosition, scaledHeight - scaledHeight, 
		//	         scaledWidth, scaledHeight);

		
		while (c.a < 1)
		{
			c.a += Time.deltaTime * UpTimeSpeed;
			splash.color = c;
			yield return null;
		}
		yield return new WaitForSeconds(WaitingTime);
		while (c.a > 0)
		{
			c.a -= Time.deltaTime * DownTimeSpeed;
			splash.color = c;
			yield return null;
		}
		Application.LoadLevel(Application.loadedLevel + 1);
	}
	
}