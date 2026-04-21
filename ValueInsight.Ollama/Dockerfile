FROM ollama/ollama:latest

ENV OLLAMA_HOST=0.0.0.0:11434

EXPOSE 11434

RUN /bin/sh -c "ollama serve & \
    sleep 15 && \
    ollama pull phi3:mini && \
    pkill ollama"

CMD ["ollama", "serve"]