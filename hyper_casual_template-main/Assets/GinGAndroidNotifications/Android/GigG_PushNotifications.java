package com.ging.gingnotifications;

import android.app.AlarmManager;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import java.util.Calendar;
import java.util.Date;


public class GigG_PushNotifications {

    private static Context mContext;
    private static GigG_PushNotifications instance;
    private String Channel = "NOTIFICATION";
    private int badgeCount = 1;
    private boolean enableVibration = true, enableLights = true;

    public static GigG_PushNotifications getInstance(Context context)
    {
        mContext = context;
        if(instance == null) instance = new GigG_PushNotifications();
        return instance;
    }

    public void NotificationOptions(String Channel, int badgeCount, boolean enableVibration, boolean enableLights)
    {
        this.Channel = Channel;
        this.badgeCount = badgeCount;
        this.enableVibration = enableVibration;
        this.enableLights = enableLights;
    }

    public void AndroidNotificationTimeIntervalTrigger(String title, String[] messages, int Minute, int Hour)
    {
        Calendar calendar = Calendar.getInstance();
        calendar.set(Calendar.HOUR_OF_DAY, Hour);
        calendar.set(Calendar.MINUTE,  Minute);

        if (calendar.getTime().compareTo(new Date()) < 0)
            calendar.add(Calendar.DAY_OF_MONTH, 1);

        Intent intent = new Intent(mContext.getApplicationContext(), NotificationReceiver.class);
        intent.putExtra("Channel", Channel);
        intent.putExtra("title", title);
        intent.putExtra("messages", messages);
        intent.putExtra("badgeCount", badgeCount);
        intent.putExtra("enableVibration", enableVibration);
        intent.putExtra("enableLights", enableLights);
        PendingIntent pendingIntent = PendingIntent.getBroadcast(mContext.getApplicationContext(), 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        AlarmManager alarmManager = (AlarmManager) mContext.getApplicationContext().getSystemService(mContext.getApplicationContext().ALARM_SERVICE);

        if (alarmManager != null) {
            alarmManager.setRepeating(AlarmManager.RTC_WAKEUP, calendar.getTimeInMillis(), AlarmManager.INTERVAL_DAY, pendingIntent);
        }
    }

    public void StopNotificationTimeIntervalTrigger()
    {
        Intent myService = new Intent(mContext.getApplicationContext(), NotificationReceiver.class);
        mContext.getApplicationContext().stopService(myService);
    }

    public void CancelNotifications()
    {
        String ns = Context.NOTIFICATION_SERVICE;
        NotificationManager nMgr = (NotificationManager) mContext.getApplicationContext().getSystemService(ns);
        nMgr.cancel(1001);
    }
}
