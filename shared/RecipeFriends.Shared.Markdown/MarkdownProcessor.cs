namespace RecipeFriends.Shared.Markdown;

public class MarkdownProcessor
{
    public string ToHtml(string markdownInput){       
        var htmlOutput = Markdig.Markdown.ToHtml(markdownInput);
        return htmlOutput;
    }
    public void ToHtml(string markdownInput, out string htmlOutput){       
        htmlOutput = Markdig.Markdown.ToHtml(markdownInput);
    }
    public void ToHtml(StreamReader markdownInput, StreamWriter htmlOutput){        
        string html = Markdig.Markdown.ToHtml(markdownInput.ReadToEnd());
        htmlOutput.Write(html);
    }
}
