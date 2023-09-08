using ObjCRuntime;
using UIKit;

namespace RecipeFriends;

public class Program
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{

        ObjCRuntime.Runtime.MarshalManagedException += (sender, args) =>
        {
            Console.WriteLine("In MarshalManagedException Handler");

            args.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
        };

        ObjCRuntime.Runtime.MarshalObjectiveCException += (sender, args) =>
        {
            Console.WriteLine("In MarshalObjectiveCException Handler");
        };

        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
