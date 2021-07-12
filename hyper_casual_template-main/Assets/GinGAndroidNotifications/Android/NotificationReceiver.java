package com.ging.gingnotifications;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class NotificationReceiver extends BroadcastReceiver {

    @Override
    public void onReceive(Context context, Intent intent) {
        String Channel = intent.getStringExtra("Channel");
        String title = intent.getStringExtra("title");
        String[] messages = intent.getStringArrayExtra("messages");
        int badgeCount = intent.getIntExtra("badgeCount", 1);
        boolean enableVibration = intent.getBooleanExtra("enableVibration", true);
        boolean enableLights = intent.getBooleanExtra("enableLights", true);

        NotificationHelper notificationHelper = new NotificationHelper(context, Channel, title, messages, badgeCount, enableVibration, enableLights);
        notificationHelper.createNotification();
    }
}