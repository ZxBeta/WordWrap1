using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class ImageUtility
{
	public static string Texture2DToBase64(Texture2D texture)
	{
		byte[] imageData = texture.EncodeToPNG();
		return Convert.ToBase64String(imageData);
	}

	public static Sprite Base64ToSprite(string encodedData)
	{
		byte[] imageData = Convert.FromBase64String(encodedData);

		int width, height;
		GetImageSize(imageData, out width, out height);

		Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
		texture.hideFlags = HideFlags.HideAndDontSave;
		texture.filterMode = FilterMode.Point;
		texture.LoadImage(imageData);

		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

		return sprite;
	}

	private static void GetImageSize(byte[] imageData, out int width, out int height)
	{
		width = ReadInt(imageData, 3 + 15);
		height = ReadInt(imageData, 3 + 15 + 2 + 2);
	}

	private static int ReadInt(byte[] imageData, int offset)
	{
		return (imageData[offset] << 8) | imageData[offset + 1];
	}

	public static bool IsBase64(this string base64String)
	{
		if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
		   || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
			return false;

		try
		{
			Convert.FromBase64String(base64String);
			return true;
		}
		catch (Exception e)
		{
			Debug.Log(e);
		}

		return false;
	}



}
