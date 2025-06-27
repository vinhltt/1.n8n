const { Client, GatewayIntentBits, Events } = require('discord.js');
const axios = require('axios');
require('dotenv').config();

// Cấu hình từ biến môi trường
const config = {
  token: process.env.DISCORD_TOKEN,
  webhookUrl: process.env.N8N_WEBHOOK_URL,
  prefix: process.env.BOT_PREFIX || '!',
  enableMessageLogging: process.env.ENABLE_MESSAGE_LOGGING === 'true',
  enableMemberJoinLogging: process.env.ENABLE_MEMBER_JOIN_LOGGING === 'true',
  guildId: process.env.GUILD_ID || null,
  logLevel: process.env.LOG_LEVEL || 'info'
};

// Tạo Discord client với các intents cần thiết
const client = new Client({
  intents: [
    GatewayIntentBits.Guilds,
    GatewayIntentBits.GuildMessages,
    GatewayIntentBits.MessageContent,
    GatewayIntentBits.GuildMembers,
    GatewayIntentBits.GuildPresences
  ],
});

// Hàm logging với level
function log(level, message, data = null) {
  const levels = { error: 0, warn: 1, info: 2, debug: 3 };
  const currentLevel = levels[config.logLevel] || 2;
  
  if (levels[level] <= currentLevel) {
    const timestamp = new Date().toISOString();
    console.log(`[${timestamp}] [${level.toUpperCase()}] ${message}`);
    if (data && config.logLevel === 'debug') {
      console.log(JSON.stringify(data, null, 2));
    }
  }
}

// Hàm gửi webhook đến n8n
async function sendWebhook(eventData) {
  if (!config.webhookUrl) {
    log('warn', 'N8N_WEBHOOK_URL không được cấu hình');
    return;
  }

  try {
    const response = await axios.post(config.webhookUrl, eventData, {
      headers: {
        'Content-Type': 'application/json',
        'User-Agent': 'Discord-N8N-Bot/1.0'
      },
      timeout: 10000 // 10 giây timeout
    });
    
    log('info', `Webhook gửi thành công: ${eventData.type}`, { 
      status: response.status,
      eventType: eventData.type 
    });
  } catch (error) {
    log('error', `Lỗi gửi webhook: ${error.message}`, {
      eventType: eventData.type,
      error: error.response?.data || error.message
    });
  }
}

// Event: Bot sẵn sàng
client.once(Events.ClientReady, (readyClient) => {
  log('info', `🤖 Bot đã online: ${readyClient.user.tag}`);
  log('info', `📊 Đang phục vụ ${readyClient.guilds.cache.size} server(s)`);
  
  // Thiết lập activity status
  client.user.setActivity('n8n workflows', { type: 'WATCHING' });
});

// Event: Tin nhắn mới
client.on(Events.MessageCreate, async (message) => {
  // Bỏ qua tin nhắn từ bot
  if (message.author.bot) return;
  
  // Kiểm tra guild ID nếu được cấu hình
  if (config.guildId && message.guild?.id !== config.guildId) return;
  
  // Chỉ log tin nhắn nếu được bật
  if (!config.enableMessageLogging) return;

  const eventData = {
    type: 'message_create',
    timestamp: new Date().toISOString(),
    message: {
      id: message.id,
      content: message.content,
      author: {
        id: message.author.id,
        username: message.author.username,
        displayName: message.author.displayName,
        bot: message.author.bot
      },
      channel: {
        id: message.channel.id,
        name: message.channel.name,
        type: message.channel.type
      },
      guild: message.guild ? {
        id: message.guild.id,
        name: message.guild.name
      } : null,
      attachments: message.attachments.map(att => ({
        id: att.id,
        name: att.name,
        url: att.url,
        size: att.size
      })),
      mentions: {
        users: message.mentions.users.map(user => ({
          id: user.id,
          username: user.username
        })),
        roles: message.mentions.roles.map(role => ({
          id: role.id,
          name: role.name
        })),
        everyone: message.mentions.everyone
      }
    }
  };

  log('debug', `Tin nhắn mới từ ${message.author.username}: ${message.content.substring(0, 50)}...`);
  await sendWebhook(eventData);
});

// Event: Thành viên mới tham gia
client.on(Events.GuildMemberAdd, async (member) => {
  // Kiểm tra guild ID nếu được cấu hình
  if (config.guildId && member.guild.id !== config.guildId) return;
  
  // Chỉ log member join nếu được bật
  if (!config.enableMemberJoinLogging) return;

  const eventData = {
    type: 'member_join',
    timestamp: new Date().toISOString(),
    member: {
      id: member.user.id,
      username: member.user.username,
      displayName: member.user.displayName,
      bot: member.user.bot,
      avatar: member.user.displayAvatarURL(),
      joinedAt: member.joinedAt,
      accountCreatedAt: member.user.createdAt
    },
    guild: {
      id: member.guild.id,
      name: member.guild.name,
      memberCount: member.guild.memberCount
    }
  };

  log('info', `Thành viên mới tham gia: ${member.user.username} vào server ${member.guild.name}`);
  await sendWebhook(eventData);
});

// Event: Thành viên rời khỏi server
client.on(Events.GuildMemberRemove, async (member) => {
  // Kiểm tra guild ID nếu được cấu hình
  if (config.guildId && member.guild.id !== config.guildId) return;

  const eventData = {
    type: 'member_leave',
    timestamp: new Date().toISOString(),
    member: {
      id: member.user.id,
      username: member.user.username,
      displayName: member.user.displayName,
      bot: member.user.bot
    },
    guild: {
      id: member.guild.id,
      name: member.guild.name,
      memberCount: member.guild.memberCount
    }
  };

  log('info', `Thành viên rời khỏi server: ${member.user.username} từ ${member.guild.name}`);
  await sendWebhook(eventData);
});

// Event: Reaction được thêm vào tin nhắn
client.on(Events.MessageReactionAdd, async (reaction, user) => {
  // Bỏ qua reaction từ bot
  if (user.bot) return;
  
  // Kiểm tra guild ID nếu được cấu hình
  if (config.guildId && reaction.message.guild?.id !== config.guildId) return;

  const eventData = {
    type: 'reaction_add',
    timestamp: new Date().toISOString(),
    reaction: {
      emoji: {
        name: reaction.emoji.name,
        id: reaction.emoji.id,
        animated: reaction.emoji.animated
      },
      count: reaction.count
    },
    user: {
      id: user.id,
      username: user.username,
      displayName: user.displayName
    },
    message: {
      id: reaction.message.id,
      author: reaction.message.author ? {
        id: reaction.message.author.id,
        username: reaction.message.author.username
      } : null,
      channel: {
        id: reaction.message.channel.id,
        name: reaction.message.channel.name
      }
    },
    guild: reaction.message.guild ? {
      id: reaction.message.guild.id,
      name: reaction.message.guild.name
    } : null
  };

  log('debug', `Reaction mới: ${user.username} react ${reaction.emoji.name}`);
  await sendWebhook(eventData);
});

// Xử lý lỗi và warnings
client.on(Events.Error, (error) => {
  log('error', 'Discord client error:', error.message);
  log('debug', 'Error details:', error);
});

client.on(Events.Warn, (warning) => {
  log('warn', 'Discord client warning:', warning);
});

client.on(Events.Debug, (info) => {
  if (config.logLevel === 'debug') {
    log('debug', `Discord debug: ${info}`);
  }
});

// Xử lý disconnect
client.on(Events.Disconnect, () => {
  log('warn', '⚠️  Bot bị disconnect từ Discord');
});

// Xử lý reconnecting  
client.on(Events.Resuming, () => {
  log('info', '🔄 Bot đang reconnecting...');
});

client.on(Events.Ready, () => {
  log('info', '✅ Bot đã reconnect thành công');
});

// Xử lý tín hiệu tắt ứng dụng
process.on('SIGINT', () => {
  log('info', 'Đang tắt bot...');
  client.destroy();
  process.exit(0);
});

process.on('SIGTERM', () => {
  log('info', 'Đang tắt bot...');
  client.destroy();
  process.exit(0);
});

// Kiểm tra cấu hình trước khi khởi động
function validateConfig() {
  log('info', 'Đang kiểm tra cấu hình...');
  
  if (!config.token) {
    log('error', 'DISCORD_TOKEN không được cấu hình trong file .env');
    log('error', 'Vui lòng thêm DISCORD_TOKEN=your_bot_token_here vào file .env');
    process.exit(1);
  }
  
  if (config.token === 'YOUR_DISCORD_TOKEN_HERE') {
    log('error', 'DISCORD_TOKEN vẫn là placeholder. Vui lòng thay bằng token thật từ Discord Developer Portal');
    process.exit(1);
  }
  
  if (!config.webhookUrl) {
    log('warn', 'N8N_WEBHOOK_URL không được cấu hình - bot sẽ chạy nhưng không gửi webhook');
  }
  
  log('info', 'Cấu hình hợp lệ');
  log('info', `Token length: ${config.token.length} chars`);
  log('info', `Token preview: ${config.token.substring(0, 10)}...`);
  log('debug', 'Webhook URL:', config.webhookUrl);
}

// Hàm khởi động bot với retry logic
async function startBot() {
  try {
    validateConfig();
    
    log('info', 'Đang khởi động Discord bot...');
    await client.login(config.token);
    
  } catch (error) {
    log('error', 'Không thể đăng nhập Discord bot:');
    log('error', `Error message: ${error.message}`);
    
    if (error.code === 'TokenInvalid') {
      log('error', '❌ Token không hợp lệ hoặc đã hết hạn');
      log('error', '💡 Vui lòng kiểm tra token tại: https://discord.com/developers/applications');
    } else if (error.code === 'DisallowedIntents') {
      log('error', '❌ Bot thiếu permissions hoặc Privileged Gateway Intents');
      log('error', '💡 Bật Message Content Intent tại Discord Developer Portal > Bot > Privileged Gateway Intents');
    } else if (error.code === 'ENOTFOUND' || error.code === 'ETIMEDOUT') {
      log('error', '❌ Không thể kết nối đến Discord. Kiểm tra kết nối mạng');
    } else {
      log('error', `❌ Lỗi không xác định: ${error.code || 'Unknown'}`);
      log('debug', 'Full error:', error);
    }
    
    // Thay vì exit ngay, đợi 5 giây rồi retry
    log('info', '🔄 Thử lại sau 5 giây...');
    setTimeout(() => {
      process.exit(1);
    }, 5000);
  }
}

// Khởi động bot
startBot(); 