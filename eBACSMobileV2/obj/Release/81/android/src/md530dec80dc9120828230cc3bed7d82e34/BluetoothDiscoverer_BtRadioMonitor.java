package md530dec80dc9120828230cc3bed7d82e34;


public class BluetoothDiscoverer_BtRadioMonitor
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("Zebra.Sdk.Printer.Discovery.BluetoothDiscoverer+BtRadioMonitor, ZebraPrinterSdk", BluetoothDiscoverer_BtRadioMonitor.class, __md_methods);
	}


	public BluetoothDiscoverer_BtRadioMonitor ()
	{
		super ();
		if (getClass () == BluetoothDiscoverer_BtRadioMonitor.class)
			mono.android.TypeManager.Activate ("Zebra.Sdk.Printer.Discovery.BluetoothDiscoverer+BtRadioMonitor, ZebraPrinterSdk", "", this, new java.lang.Object[] {  });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

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
