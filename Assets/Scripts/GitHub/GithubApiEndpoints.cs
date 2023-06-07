using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GithubApiEndpoints 
{
    public static string AUTHENTICATE = "/user";

    public static string GET_COMMITS = "/repos/{0}/{1}/commits"; // owner, repo
    public static string GET_COMMIT = "/repos/{0}/{1}/commits/{2}"; // owner, repo,

    public static string GET_PROJECT_LIST = "/user/repos"; // for authenticated users
    public static string GET_PROJECT_LIST_PUBLIC = "/{0}/repos"; // user; use this string when no token is provided
    public static string GET_COMMIT_LIST = "/repos/{0}/{1}/commits?per_page=10"; // owner, repo

}
