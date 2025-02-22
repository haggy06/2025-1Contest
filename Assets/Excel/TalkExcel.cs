using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class TalkExcel : ScriptableObject
{
	public List<TalkSheet> StoryTalk; // Replace 'EntityType' to an actual type that is serializable.
	public List<TalkSheet> NPCTalk; // Replace 'EntityType' to an actual type that is serializable.
    public List<TalkSheet> InfoTalk;
}
