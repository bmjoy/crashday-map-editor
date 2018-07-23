//--------------------------------------//               PowerUI////        For documentation or //    if you have any issues, visit//        powerUI.kulestar.com////    Copyright � 2013 Kulestar Ltd//          www.kulestar.com//--------------------------------------using System;using InfiniText;namespace Css.Units{		/// <summary>	/// Represents an instance of ex units.	/// </summary>		public class ExUnit:FontUnit{				public override string ToString(){			return RawValue+"ex";		}				public override float GetDecimal(RenderableData context,CssProperty property){						// Get the active font face:			FontFace activeFont=GetFontFace(context);						float ex;						if(activeFont==null){				ex=0.5f;			}else{				ex=activeFont.ExHeight;			}						return RawValue * ex * GetFontSize(context,property) * context.ValueScaleRelative;					}				protected override Value Clone(){			ExUnit result=new ExUnit();			result.RawValue=RawValue;			return result;		}				public override string[] PostText{			get{				return new string[]{"ex"};			}		}			}	}