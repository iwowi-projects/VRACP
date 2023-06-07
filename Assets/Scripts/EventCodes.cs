using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventCodes
{
    // Configuration Area
    public const byte ClearAll = 1;

    // Backlog area
    public const byte SynchronizeBacklogPage = 10;
    public const byte SetupBacklogShelf = 11;

    // Board area
    public const byte SetupBoardColumns = 20;
    public const byte SetupBoardColumn = 21;
    public const byte SynchronizeBoardColumnPage = 22;
    public const byte ClearBoardColumns = 23;

    // Sprints area
    public const byte SetupSprintShelf = 30;
    public const byte SynchronizeSprintPage = 31;

    // Sprint Issues area
    public const byte SetupSprintIssuesShelf= 40;
    public const byte SynchronizeSprintIssuesPage = 41;

    
    // BaseEditCreateCard
    public const byte SynchronizeBaseEditCreateCardOpen = 50;
    public const byte SynchronizeBaseEditCreateCardClose = 51;
    public const byte SynchronizeEditIssueCardPriority = 52;


    // Issue Card
    public const byte SynchronizeIssueCard = 60;
    public const byte SyncIssueCardHolding = 61;

    // Inputfield, dropdown, textfield
    public const byte SynchronizeTextFieldText = 70;
    public const byte SynchronizeInputFieldText = 71;
    public const byte SynchronizeDropdownValue = 72;
    public const byte SynchronizeDropdownOptions = 73;
    public const byte SynchronizeDropdownOpen = 74;
    public const byte SynchronizeToggleState = 75;


    // Github Integration
    public const byte SynchronizeGitHubFile = 83;

}
