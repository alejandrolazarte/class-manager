export interface TeamNotification {
  id: string;
  title: string;
  body: string;
  url: string;
  createdAt: string;
  isUnread: boolean;
}

export interface TeamNotifications {
  items: TeamNotification[];
  unreadCount: number;
}

export interface TeamPushKey {
  publicKey: string | null;
}
