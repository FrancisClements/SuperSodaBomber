/// Credit Melang
/// Sourced from - http://forum.unity3d.com/members/melang.593409/
/// Updated by @DanjelRicci on 2019-03-11 - This version removes the continuous GC spikes of the original script.

/*
Nicer Outline
    A component that gives the text an ability to
    have an outline/stroke.
*/

using System.Collections.Generic;
namespace UnityEngine.UI.Extensions{
	//An outline that looks a bit nicer than the default one. It has less "holes" in the outline by drawing more copies of the effect
	[AddComponentMenu("UI/Effects/Extensions/Nicer Outline")]
	public class NicerOutline : BaseMeshEffect
	{
		[SerializeField]
		private Color m_EffectColor = new Color (0f, 0f, 0f, 0.5f);
		
		[SerializeField]
		private Vector2 m_EffectDistance = new Vector2 (1f, -1f);
		
		[SerializeField]
		private bool m_UseGraphicAlpha = true;
		//
		// Properties
		//
		public Color effectColor
		{
			get
			{
				return this.m_EffectColor;
			}
			set
			{
				this.m_EffectColor = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}
		
		public Vector2 effectDistance
		{
			get
			{
				return this.m_EffectDistance;
			}
			set
			{
				if (value.x > 600f)
				{
					value.x = 600f;
				}
				if (value.x < -600f)
				{
					value.x = -600f;
				}
				if (value.y > 600f)
				{
					value.y = 600f;
				}
				if (value.y < -600f)
				{
					value.y = -600f;
				}
				if (this.m_EffectDistance == value)
				{
					return;
				}
				this.m_EffectDistance = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}
		
		public bool useGraphicAlpha
		{
			get
			{
				return this.m_UseGraphicAlpha;
			}
			set
			{
				this.m_UseGraphicAlpha = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty ();
				}
			}
		}

		UIVertex vt;
		int neededCpacity;
		Color newColor;
        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                Vector3 v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;
                newColor = color;
                if (m_UseGraphicAlpha)
                    newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
                vt.color = newColor;
                verts[i] = vt;
            }
        }

        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }

		List<UIVertex> verts = new List<UIVertex>();
		Text foundtext;
		float best_fit_adjustment;
		float distanceX;
		float distanceY;
		int start;
		int count;
		public override void ModifyMesh (VertexHelper vh) {
			if (!this.IsActive()) {
				return;
			}
			verts.Clear();
			vh.GetUIVertexStream(verts);

			if (!foundtext) {
				foundtext = GetComponent<Text>();
			}
			
			best_fit_adjustment = 1f;
			
			if (foundtext && foundtext.resizeTextForBestFit)  
			{
				best_fit_adjustment = (float)foundtext.cachedTextGenerator.fontSizeUsedForBestFit / (foundtext.resizeTextMaxSize-1); //max size seems to be exclusive 

			}

			distanceX = this.effectDistance.x * best_fit_adjustment;
			distanceY = this.effectDistance.y * best_fit_adjustment;

			start = 0;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, distanceX, distanceY);
			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, distanceX, -distanceY);
			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, -distanceX, distanceY);
			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, -distanceX, -distanceY);

			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, distanceX, 0);
			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, -distanceX, 0);

			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, 0, distanceY);
			start = count;
			count = verts.Count;
			this.ApplyShadow (verts, this.effectColor, start, verts.Count, 0, -distanceY);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }

#if UNITY_EDITOR
		protected override void OnValidate ()
		{
			this.effectDistance = this.m_EffectDistance;
			base.OnValidate ();
		}
#endif
	}
}