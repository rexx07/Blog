namespace Blog.Data;

public enum GroupAction
{
    Publish,
    Unpublish,
    Feature,
    Delete
}

public enum PublishedStatus
{
    All,
    Published,
    Drafts,
    Featured
}

public enum ArticleListType
{
    Category,
    User,
    Search
}

public enum ArticleAction
{
    Save,
    Publish,
    Unpublish
}

public enum ArticleType
{
    Article = 1,
    Page = 2
}

public enum SaveStatus
{
    Saving = 1,
    Publishing = 2,
    Unpublishing = 3
}

public enum UploadType
{
    Avatar,
    Attachement,
    AppLogo,
    AppCover,
    ArticleCover,
    ArticleImage
}

public enum Status
{
    Success,
    Warning,
    Error
}

public enum ImportType
{
    Post,
    Image,
    Attachement
}

public enum AnalyticsListType
{
    List = 1,
    Graph = 2
}

public enum AnalyticsPeriod
{
    Today = 1,
    Yesterday = 2,
    Days7 = 3,
    Days30 = 4,
    Days90 = 5
}