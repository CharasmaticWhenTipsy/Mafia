## Frontend
# Stage 1: Build the React app
FROM node:21.6 AS build_frontend

# Set the working directory inside the container
WORKDIR /app/Mafia/frontend

# Install dependencies
COPY package*.json ./

RUN npm install

# Copy the source code into the container
COPY . .

# Build the React app for production
RUN npm run build

# Stage 2: Serve the React app
FROM nginx:alpine as final_frontend

# Copy the build output from the previous stage
COPY --from=build_frontend /app/Mafia/frontend/dist /usr/share/nginx/html

# Expose the port the app will run on
EXPOSE 80

# Start the Nginx server
CMD ["nginx", "-g", "daemon off;"]