package md535db48e1d5bba9ef3795e382af371424;


public class ReadingActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("eBACSMobileV2.ReadingActivity, eBACSMobileV2", ReadingActivity.class, __md_methods);
	}


	public ReadingActivity ()
	{
		super ();
		if (getClass () == ReadingActivity.class)
			mono.android.TypeManager.Activate ("eBACSMobileV2.ReadingActivity, eBACSMobileV2", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
