username=${1:-${USERNAME}}
hostname=${2:-${HOSTNAME}}

echo "Testing ssh connection"

ssh $username@$hostname -o StrictHostKeyChecking=no "ls"