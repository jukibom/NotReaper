using System.Linq;
using System.Collections.Generic;
using NotReaper.Grid;
using NotReaper.Targets;
using NotReaper.UserInput;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotReaper.Tools {


	public class UndoRedoManager : MonoBehaviour {

		/// <summary>
		/// Contains the complete list of actions the user has done recently.
		/// </summary>
		public List<NRAction> actions = new List<NRAction>();

		/// <summary>
		/// Contains the actions the user has "undone" for future use.
		/// </summary>
		public List<NRAction> redoActions = new List<NRAction>();

		public int maxSavedActions = 20;
		




		public Timeline timeline;

		public void Undo() {
			if (actions.Count <= 0) return;

			NRAction action = actions.Last();
			Debug.Log("Undoing action:" + action.type.ToString());

			InvertAction(action);
			try {


			} catch {
				Debug.LogError("There was an error trying to undo.");
			}

			
			redoActions.Add(action);
			actions.RemoveAt(actions.Count - 1);

			
		}

		public void Redo() {

			if (redoActions.Count <= 0) return;

			NRAction action = redoActions.Last();

			Debug.Log("Redoing action:" + action.type.ToString());

				RunAction(action);
			try {
			} catch {
				Debug.LogError("There was an error trying to redo.");
			}

			//We remove the old action and just add a new one from Timeline.cs since the old one lost the reference to it's GO's
			//AddAction(action, false);
			redoActions.RemoveAt(redoActions.Count - 1);
			

		}

		//Use for undo (DON'T generate undo actions from these)
		public void InvertAction(NRAction action) {

			switch (action.type) {
				
				case ActionType.AddNote:
					//Might crash if it tries to delete non-loaded note in loadednotes list
					timeline.DeleteTarget(action.affectedTarget, false);
					break;

				case ActionType.MultiAddNote:
					timeline.DeleteTargets(action.affectedTargets, false);
					break;

				case ActionType.RemoveNote:
					//Re-add the target based on all the previous stats from the target;
					Target tar = action.affectedTarget;
					timeline.AddTarget(tar.gridTargetPos.x, tar.gridTargetPos.y, tar.gridTargetPos.z, false, false, tar.beatLength, tar.velocity, tar.handType, tar.behavior);
					break;

				case ActionType.MultiRemoveNote:
				
					timeline.AddTargets(action.affectedTargets, false);

					break;
				

				default:
					break;
				

			}


		}


		//Use for redo, DO generate undo actions, and don't clear redo actions
		public void RunAction(NRAction action) {
			switch (action.type) {
				case ActionType.AddNote:
					timeline.AddTarget(action.affectedTarget, true);
					break;

				case ActionType.MultiAddNote:
					timeline.AddTargets(action.affectedTargets, true);
					break;

				case ActionType.RemoveNote:
					//Might crash if it tries to delete non-loaded note in loadednotes list
					timeline.DeleteTarget(action.affectedTarget, true, false);
					break;

				case ActionType.MultiRemoveNote:
					timeline.DeleteTargets(action.affectedTargets, true, false);
					break;




			}
		}



		public void AddAction(NRAction action, bool clearRedoActions = true) {
			if (actions.Count <= maxSavedActions) {
				actions.Add(action);
			} else {
				while (maxSavedActions > actions.Count) {
					actions.RemoveAt(0);
				}

				actions.Add(action);
			}

			if (clearRedoActions) redoActions = new List<NRAction>();



		}






		
	}


	public enum ActionType {
		AddNote = 0,
		MultiAddNote = 1,
		RemoveNote = 2,
		MultiRemoveNote = 3,
		SelectNote = 4,
		MultiSelectNote = 5,
		DeselectNote = 6,
		MultiDeselectNote = 7,
		MoveNote = 8,
		MultiMoveNote = 9
		
	}

	public class NRAction {
		public ActionType type;
		public List<Target> affectedTargets = new List<Target>();
		public Target affectedTarget {
			get {
				return affectedTargets.First();
			}
		}

		//public Vector2 redoTargetPos;

		//public Vector2 movePreviousPos;

		

	}


}