package com.ging.gingnotifications;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;

import java.util.Random;

class NotificationHelper {

    private Context mContext;
    private String Channel;
    private String Title;
    private String[] Messages;
    private int badgeCount;
    private boolean enableVibration;
    private boolean enableLights;
    private static final String NOTIFICATION_CHANNEL_ID = "10001";

    NotificationHelper(Context context, String Channel, String Title, String[] Messages, int badgeCount, boolean enableVibration, boolean enableLights) {
        mContext = context;
        this.Channel = Channel;
        this.Title = Title;
        this.Messages = Messages;
        this.badgeCount = badgeCount;
        this.enableVibration = enableVibration;
        this.enableLights = enableLights;
    }

    void createNotification()
    {
        String packageName = mContext.getApplicationContext().getPackageName();
        Intent launchIntent = mContext.getApplicationContext().getPackageManager().getLaunchIntentForPackage(packageName);
        launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        PendingIntent resultPendingIntent = PendingIntent.getActivity(mContext.getApplicationContext(), 0, launchIntent, PendingIntent.FLAG_UPDATE_CURRENT);
        Notification notification = null;

        NotificationManager mNotificationManager = (NotificationManager) mContext.getApplicationContext().getSystemService(Context.NOTIFICATION_SERVICE);
        int IndexMessage = new Random().nextInt(Messages.length);

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O)
        {
            int importance = NotificationManager.IMPORTANCE_HIGH;
            NotificationChannel notificationChannel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, Channel, importance);
            notificationChannel.enableLights(enableLights);
            notificationChannel.setLightColor(Color.RED);
            notificationChannel.enableVibration(enableVibration);
            notificationChannel.setVibrationPattern(new long[]{100, 200, 300, 400, 500, 400, 300, 200, 400});

            notification = new Notification.Builder(mContext.getApplicationContext())
                    .setContentTitle(Title)
                    .setContentText(Messages[IndexMessage])
                    .setSmallIcon(mContext.getApplicationContext().getApplicationInfo().icon)
                    .setAutoCancel(false)
                    .setNumber(badgeCount)
                    .setContentIntent(resultPendingIntent)
                    .setChannelId(NOTIFICATION_CHANNEL_ID)
                    .build();

            assert mNotificationManager != null;
            mNotificationManager.createNotificationChannel(notificationChannel);
        } else
        {
            notification = new Notification.Builder(mContext.getApplicationContext())
                    .setContentTitle(Title)
                    .setContentText(Messages[IndexMessage])
                    .setSmallIcon(mContext.getApplicationContext().getApplicationInfo().icon)
                    .setAutoCancel(false)
                    .setNumber(badgeCount)
                    .setContentIntent(resultPendingIntent)
                    .build();
        }

        NotificationManager notificationManager = (NotificationManager) mContext.getApplicationContext().getSystemService(mContext.getApplicationContext().NOTIFICATION_SERVICE);
        notificationManager.notify(1001, notification);
    }
}