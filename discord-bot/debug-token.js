const { Client, GatewayIntentBits } = require('discord.js');
require('dotenv').config();

console.log('🔍 Discord Token Debug Script');
console.log('==============================');

const token = process.env.DISCORD_TOKEN;

if (!token) {
  console.log('❌ DISCORD_TOKEN không được cấu hình');
  console.log('📝 Vui lòng kiểm tra file .env hoặc biến môi trường');
  process.exit(1);
}

console.log(`✅ DISCORD_TOKEN được tìm thấy`);
console.log(`📏 Độ dài token: ${token.length} ký tự`);
console.log(`🔑 Token preview: ${token.substring(0, 10)}...${token.substring(token.length - 10)}`);

// Kiểm tra format cơ bản của Discord token
const tokenRegex = /^[A-Za-z0-9_-]{24}\.[A-Za-z0-9_-]{6}\.[A-Za-z0-9_-]{27}$/;
if (tokenRegex.test(token)) {
  console.log('✅ Token có format hợp lệ');
} else {
  console.log('⚠️ Token có thể không đúng format Discord bot token');
}

// Thử đăng nhập để kiểm tra token
console.log('\n🔄 Đang thử đăng nhập...');

const client = new Client({
  intents: [GatewayIntentBits.Guilds]
});

client.once('ready', () => {
  console.log(`✅ Đăng nhập thành công: ${client.user.tag}`);
  console.log(`🤖 Bot ID: ${client.user.id}`);
  console.log(`📊 Đang phục vụ ${client.guilds.cache.size} server(s)`);
  
  client.guilds.cache.forEach(guild => {
    console.log(`  - ${guild.name} (${guild.id})`);
  });
  
  client.destroy();
  console.log('\n✅ Test hoàn thành - Token hợp lệ!');
  process.exit(0);
});

client.on('error', (error) => {
  console.log('❌ Lỗi Discord client:', error.message);
  process.exit(1);
});

// Timeout sau 10 giây
setTimeout(() => {
  console.log('⏰ Timeout - không thể kết nối sau 10 giây');
  process.exit(1);
}, 10000);

client.login(token).catch(error => {
  console.log('❌ Không thể đăng nhập:', error.message);
  
  if (error.code === 'TokenInvalid') {
    console.log('🔑 Token không hợp lệ - vui lòng kiểm tra lại');
  } else if (error.code === 'DisallowedIntents') {
    console.log('🚫 Bot thiếu quyền intents - kiểm tra Discord Developer Portal');
  }
  
  process.exit(1);
}); 