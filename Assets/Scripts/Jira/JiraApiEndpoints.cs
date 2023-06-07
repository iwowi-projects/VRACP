using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JiraApiEndpoints
{

    // BASIC
    public const string GET_MYSELF = "/rest/api/latest/myself";
    public const string GET_ALL_PROJECTS = "/rest/api/latest/project?expand=issueTypes";
    public const string GET_ALL_ISSUE_TYPES = "/rest/api/latest/issuetype";


    // BOARDS
    public const string GET_ALL_BOARDS = "/rest/agile/1.0/board";
    public const string GET_CONFIGURATION_FOR_BOARD = "/rest/agile/1.0/board/{0}/configuration";
    public const string GET_FEATURES_FOR_BOARD = "/rest/agile/1.0/board/{0}/features";
    public const string GET_PROJECTS_FOR_BOARD = "/rest/agile/1.0/board/{0}/project?expand=issueTypes";

    public const string GET_BACKLOG_FOR_BOARD = "/rest/agile/1.0/board/{0}/backlog";
    public const string GET_ISSUES_FOR_BOARD = "/rest/agile/1.0/board/{0}/issue";
    public const string GET_SPRINTS_FOR_BOARD = "/rest/agile/1.0/board/{0}/sprint?state=future,active";
    public const string GET_ACTIVE_SPRINTS_FOR_BOARD = "/rest/agile/1.0/board/{0}/sprint?state=active";
    public const string GET_CLOSED_SPRINTS_FOR_BOARD = "/rest/agile/1.0/board/{0}/sprint?state=closed";
    public const string GET_ISSUES_FOR_BOARD_AND_SPRINT = "/rest/agile/1.0/board/{0}/sprint/{1}/issue";
    

    // SPRINTS
    public const string GET_SPRINT = "/rest/agile/1.0/sprint/{0}";
    public const string GET_SPRINT_ISSUES = "/rest/agile/1.0/sprint/{0}/issue";
    public const string CREATE_SPRINT = "/rest/agile/1.0/sprint";
    public const string UPDATE_SPRINT = "/rest/agile/1.0/sprint/{0}";
    public const string MOVE_ISSUE_TO_SPRINT = "/rest/agile/1.0/sprint/{0}/issue";


    // ISSUES
    public const string GET_ISSUE = "/rest/api/latest/issue/{0}";
    public const string CREATE_ISSUE = "/rest/api/latest/issue";

    public const string EDIT_ISSUE = "/rest/api/latest/issue/{0}";
    public const string GET_ISSUE_EDITMETA = "/rest/api/latest/issue/{0}/editmeta";
    public const string GET_ISSUE_TRANSITIONS = "/rest/api/latest/issue/{0}/transitions";
    public const string POST_ISSUE_TRANSITION = "/rest/api/latest/issue/{0}/transitions";
    
    public const string DELETE_ISSUE = "/rest/api/latest/issue/{0}";
    
    public const string MOVE_ISSUE_TO_BACKLOG = "/rest/agile/1.0/backlog/issue";
    public const string MOVE_ISSUE_TO_BOARD = "/rest/agile/1.0/board/{0}/issue";

}
